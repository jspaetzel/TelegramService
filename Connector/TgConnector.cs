using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLChatFull = TeleSharp.TL.Messages.TLChatFull;

namespace TelegramService.Connector
{
    public class TgConnector : ITgConnector
    {
        private readonly TelegramClient client;
        private readonly string number;


        public TgConnector()
        {
			number = WebConfigurationManager.AppSettings["Number"];
			var apiId = int.Parse(WebConfigurationManager.AppSettings["ApiId"]);
			var apiHash = WebConfigurationManager.AppSettings["ApiHash"];

            try
            {
                client = new TelegramClient(apiId, apiHash, new RelativeFileSessionStore());
            }
            catch (MissingApiConfigurationException ex)
            {
                throw new Exception("Please add your API settings to the `app.config` file. (More info: " + MissingApiConfigurationException.InfoUrl + ")",ex);
            }
        }

        public async Task<ConnectResponse> Connect()
        {
            var response = new ConnectResponse();

            await client.ConnectAsync();

            if (!client.IsUserAuthorized())
            {
                var hash = await client.SendCodeRequestAsync(number);

                response.Status = ConnectorStatus.NotAuthorized;
                response.AuthCode = hash;
                return response;
            }

            response.Status = ConnectorStatus.Connected;
            return response;
        }

        public async Task<TLUser> Authorize(int code, string hash)
        {
            try
            {
                await client.ConnectAsync();

                var user = await client.MakeAuthAsync(number, hash, code.ToString());
                return user;
            }
            catch (InvalidPhoneCodeException ex)
            {
                throw new Exception(
                    "CodeToAuthenticate is wrong in the app.config file, fill it with the code you just got now by SMS/Telegram",
                    ex);
            }
        }

        public async Task<bool> SendChatMessage(int chatId, string message)
        {
            await client.ConnectAsync();

            var peer = new TLInputPeerChat
            {
                chat_id = chatId
            };

            try
            {
                 await client.SendMessageAsync(peer, message);
                 return true;
            }
            catch (InvalidOperationException e)
            {
                return false;
            }
        }

        public async Task<TLVector<TLAbsUser>> GetUserContacts()
        {
            await client.ConnectAsync();

            var contacts = await client.GetContactsAsync();

            return contacts.users;
        }

        public async Task<TLVector<TLContact>> GetContacts()
        {
            await client.ConnectAsync();

            var contacts = await client.GetContactsAsync();

            return contacts.contacts;
        }

        public async Task<TLChats> GetChats()
        {
            await client.ConnectAsync();
            var dialogs = (TLDialogs) await client.GetUserDialogsAsync();

            var chatIds = dialogs.chats.lists
                .Where(c => c.GetType() == typeof(TLChat))
                .Cast<TLChat>()
                .Select(x => x.id)
                .ToList();

            var request = new TLRequestGetChats
            {
                id = new TLVector<int>
                {
                    lists = chatIds
                }
            };

            var chats = await client.SendRequestAsync<TLChats>(request);
            return chats;
        }

        public async Task<ConnectorStatus> GetStatus()
        {
            try
            {
                await client.ConnectAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connecting: " + e.InnerException);
                return ConnectorStatus.ClientError;
            }

            var registered = await client.IsPhoneRegisteredAsync(number);
            if (!registered)
                return ConnectorStatus.NotRegistered;

            if (!client.IsUserAuthorized())
                return ConnectorStatus.NotAuthorized;

            return ConnectorStatus.Connected;
        }

        public async Task<TLChat> CreateChat(int userId, long accessHash, string title)
        {
            await client.ConnectAsync();

            TLRequestCreateChat req = new TLRequestCreateChat
            {
                title = title,
                users = new TLVector<TLAbsInputUser>
                {
                    lists = new List<TLAbsInputUser>
                    {
                        new TLInputUser {
                            user_id = userId,
                            access_hash = accessHash
                        }
                    }
                }
            };

            TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);

            var chat = result.chats.lists
                .Where(c => c.GetType() == typeof(TLChat))
                .Cast<TLChat>()
                .FirstOrDefault();

            if (chat == null)
            {
                throw new Exception("Error creating chat");
            }
            await this.ToggleChatAdmin(chat.id, false);
            return chat;
        }

        public async Task<bool> EditChatAdmin(int chatId, int userId, long accessHash, bool isAdmin)
        {
            await client.ConnectAsync();

            var request = new TLRequestEditChatAdmin
            {
                chat_id = chatId,
                is_admin = isAdmin,
                user_id = new TLInputUser
                {
                    user_id = userId,
                    access_hash = accessHash
                }
            };

            return await client.SendRequestAsync<bool>(request);
        }

        public async Task<bool> EditChannelUserRole(int channelId, long channelHash, int userId, long userHash, RoleEnum role)
        {
            await client.ConnectAsync();

            TLAbsChannelParticipantRole channelRole;
            switch (role)
            {
                case RoleEnum.None:
                    channelRole = new TLChannelRoleEmpty();
                    break;
                case RoleEnum.Editor:
                    channelRole = new TLChannelRoleEditor();
                    break;
                case RoleEnum.Mod:
                    channelRole = new TLChannelRoleModerator();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }

            var request = new TLRequestEditAdmin
            {
                channel = new TLInputChannel
                {
                    access_hash = channelHash,
                    channel_id = channelId
                },
                user_id = new TLInputUser
                {
                    access_hash = userHash,
                    user_id = userId
                },
                role = channelRole
            };

            return await client.SendRequestAsync<bool>(request);
        }

        public async Task<TLChannelParticipants> GetChannelParticipants(int channelId, long channelHash, FilterEnum? filterEnum, int limit, int offset)
        {
            await client.ConnectAsync();

            TLAbsChannelParticipantsFilter filter;
            switch (filterEnum)
            {
                case FilterEnum.Admins:
                    filter = new TLChannelParticipantsAdmins();
                    break;
                case FilterEnum.Bots:
                    filter = new TLChannelParticipantsBots();
                    break;
                case FilterEnum.Kicked:
                    filter = new TLChannelParticipantsKicked();
                    break;
                case FilterEnum.Recent:
                    filter = new TLChannelParticipantsRecent();
                    break;
                default:
                    filter = null;
                    break;
            }

            TLRequestGetParticipants particpants = new TLRequestGetParticipants
            {
                channel = new TLInputChannel
                {
                    access_hash = channelHash,
                    channel_id = channelId
                },
                filter = filter,
                limit = limit,
                offset = offset
            };

            return await client.SendRequestAsync<TLChannelParticipants>(particpants);
        }



        /// <summary>
        /// Returns true if successful
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="allMembersAreAdmins"></param>
        /// <returns>bool to indicate success</returns>
        public async Task<bool> ToggleChatAdmin(int chatId, bool allMembersAreAdmins)
        {
            await client.ConnectAsync();

            var request = new TLRequestToggleChatAdmins
            {
                chat_id = chatId,
                enabled = !allMembersAreAdmins // enabled is opposite the boolean
            };
            try
            {
                await client.SendRequestAsync<TLAbsUpdates>(request);
                return true;
            }
            catch (InvalidOperationException e)
            {
                if (e.Message == "CHAT_NOT_MODIFIED")
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<TLAbsMessages> GetHistory(int id, long? hash, int limit, int offset, int maxId, HistoryPeer historyPeer)
        {
            await client.ConnectAsync();

            TLAbsInputPeer peer = null;
            switch (historyPeer)
            {
                case HistoryPeer.Channel:
                    if (hash == null)
                    {
                        throw new NullReferenceException("hash");
                    }
                    peer = new TLInputPeerChannel {
                        channel_id = id,
                        access_hash = hash.Value
                    };
                    break;
                case HistoryPeer.Chat:
                    peer = new TLInputPeerChat {
                        chat_id = id
                    };
                    break;
                case HistoryPeer.User:
                    if (hash == null)
                    {
                        throw new NullReferenceException("hash");
                    }
                    peer = new TLInputPeerUser {
                        user_id = id,
                        access_hash = hash.Value
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(historyPeer), historyPeer, null);
            }

            return await client.GetHistoryAsync(peer, offset, maxId, limit);
        }

        public async Task<bool> RemoveChatUser(int chatId, int userId, long accessHash)
        {
            await client.ConnectAsync();

            TLRequestDeleteChatUser req = new TLRequestDeleteChatUser
            {
                chat_id = chatId,
                user_id = new TLInputUser
                {
                    user_id = userId,
                    access_hash = accessHash
                }
            };

            try
            {
                TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddChatUser(int chatId, int userId, long accessHash, int forwardMessageCount = 0)
        {

            await client.ConnectAsync();

            TLRequestAddChatUser req = new TLRequestAddChatUser
            {
                chat_id = chatId,
                user_id = new TLInputUser
                {
                    user_id = userId,
                    access_hash = accessHash
                },
                fwd_limit = forwardMessageCount
            };

            try
            {
                TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RenameChat(int chatId, string newName)
        {

            await client.ConnectAsync();

            TLRequestEditChatTitle req = new TLRequestEditChatTitle
            {
                chat_id = chatId,
                title = newName
            };

            TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);

            var chat = result.chats.lists
                .Where(c => c.GetType() == typeof(TLChat))
                .Cast<TLChat>()
                .FirstOrDefault();

            if (chat == null)
            {
                throw new Exception("Error renaming chat");
            }
            return true;
        }

        public async Task<TLVector<TLAbsUser>> GetMembers(int chatId)
        {
            await client.ConnectAsync();
            TLRequestGetFullChat req = new TLRequestGetFullChat
            {
                chat_id = chatId
            };

            TLChatFull result = await client.SendRequestAsync<TLChatFull>(req);
            return result.users;
        }

        public async Task<TLAbsUser> ResolveUsername(string username)
        {
            await client.ConnectAsync();

            var req = new TLRequestResolveUsername
            {
                username = username
            };

            TLResolvedPeer result = await client.SendRequestAsync<TLResolvedPeer>(req);
            var list = result.users.lists.ToList();
            var user = list.FirstOrDefault();
            return user;
        }

        public async Task<TLUpdates> CreateChannel(string title, string about, bool megaGroup, bool broadcast)
        {
            await client.ConnectAsync();

            TLRequestCreateChannel req = new TLRequestCreateChannel
            {
                title = title,
                about = about,
                megagroup = megaGroup,
                broadcast = broadcast
            };

            TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);
            return result;
        }

        public async Task<bool> AddChannelUser(int channelId, long channelHash, int userId, long userHash)
        {
            await client.ConnectAsync();

            TLRequestInviteToChannel req = new TLRequestInviteToChannel
            { 
                channel = new TLInputChannel
                {
                    access_hash = channelHash,
                    channel_id = channelId
                },
                users = new TLVector<TLAbsInputUser>
                {
                    lists = new List<TLAbsInputUser>
                    {
                        new TLInputUser
                        {
                            access_hash = userHash,
                            user_id = userId
                        }
                    }
                }
            };

            try
            {
                TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveChannelUser(int channelId, long channelHash, int userId, long userHash, bool kicked)
        {
            await client.ConnectAsync();

            TLRequestKickFromChannel req = new TLRequestKickFromChannel
            {
                channel = new TLInputChannel
                {
                    access_hash = channelHash,
                    channel_id = channelId
                },
                user_id = new TLInputUser
                {
                    access_hash = userHash,
                    user_id = userId
                },
                kicked = kicked
            };

            try
            {
                TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChatMigrate(int chatId)
        {
            await client.ConnectAsync();

            TLRequestMigrateChat req = new TLRequestMigrateChat
            {
                chat_id = chatId
            };

            try
            {
                TLUpdates result = await client.SendRequestAsync<TLUpdates>(req);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}