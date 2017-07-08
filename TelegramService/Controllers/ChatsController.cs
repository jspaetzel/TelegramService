using System.Threading.Tasks;
using System.Web.Http;
using TelegramService.Connector;
using TelegramService.Filters;
using TelegramService.Models;
using TeleSharp.TL;

namespace TelegramService.Controllers
{
    public class ChatsController : ApiController
    {
        private readonly TgConnector clientConnector;

        public ChatsController()
        {
            this.clientConnector = new TgConnector();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats")]
        public async Task<TLChat> CreateChat([FromBody] ChatCreateRequest request)
        {
            return await clientConnector.CreateChat(request.UserId, request.UserHash, request.Title);
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/admin-toggle")]
        public async Task<IHttpActionResult> ToggleAdmin(int chatId, [FromBody] ChatToggleAdminRequest request)
        {
            var result = await clientConnector.ToggleChatAdmin(chatId, request.AllMembersAreAdmins);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/send-message")]
        public async Task<IHttpActionResult> SendMessage(int chatId, [FromBody] ChatSendMessageRequest request)
        {
            var result = await clientConnector.SendChatMessage(chatId, request.Message);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/is-admin")]
        public async Task<IHttpActionResult> UserChatAdmin(int chatId, [FromBody] ChatUserAdminRequest request)
        {
            var result = await clientConnector.EditChatAdmin(chatId, request.UserId, request.UserHash, request.IsAdmin);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/add")]
        public async Task<IHttpActionResult> AddUser(int chatId, [FromBody] ChatAddUserRequest request)
        {
            var result = await clientConnector.AddChatUser(chatId, request.UserId, request.UserHash, request.ForwardMessageCount);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/remove")]
        public async Task<IHttpActionResult> RemoveUser(int chatId, [FromBody] ChatRemoveUserRequest request)
        {
            var result = await clientConnector.RemoveChatUser(chatId, request.UserId, request.UserHash);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/rename")]
        public async Task<IHttpActionResult> RenameChat(int chatId, [FromBody] ChatRenameRequest request)
        {
            var result = await clientConnector.RenameChat(chatId, request.NewName);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("chats/{chatId}/history")]
        public async Task<IHttpActionResult> GetHistory(int chatId, int limit = 0, int offset = 0, int maxId = -1)
        {
            var result = await clientConnector.GetHistory(chatId, null, limit, offset, maxId, Connector.HistoryPeer.Chat);

            if (result != null)
            {
                return this.Ok(result);
            }
            return this.InternalServerError();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("chats/{chatId}/members")]
        public async Task<IHttpActionResult> GetMembers(int chatId)
        {
            var result = await clientConnector.GetMembers(chatId);

            if (result != null)
            {
                return this.Ok(result);
            }
            return this.InternalServerError();
        }


        [HttpPost]
        [AuthorizeKey]
        [Route("chats/{chatId}/migrate")]
        public async Task<IHttpActionResult> ChatMigrate(int chatId)
        {
            var result = await clientConnector.ChatMigrate(chatId);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }
    }
}