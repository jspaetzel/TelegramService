using System.Threading.Tasks;
using System.Web.Http;
using TelegramService.Connector;
using TelegramService.Filters;
using TelegramService.Models;
using TeleSharp.TL;

namespace TelegramService.Controllers
{
    public class ChannelsController : ApiController
    {
        private readonly TgConnector clientConnector;

        public ChannelsController()
        {
            this.clientConnector = new TgConnector();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("channels")]
        public async Task<TLUpdates> CreateChannel([FromBody] ChannelCreateRequest request)
        {
            return await clientConnector.CreateChannel(request.Title, request.About, request.MegaGroup, request.Broadcast);
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("channels/{channelId}/users/{userId}/role")]
        public async Task<IHttpActionResult> UserChatAdmin(int channelId, int userId, [FromBody] ChannelUserRoleRequest request)
        {
            var result = await clientConnector.EditChannelUserRole(channelId, request.ChannelHash, userId, request.UserHash, request.Role);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("channels/{channelId}")]
        public async Task<IHttpActionResult> UserChatAdmin(int channelId, long channelHash, FilterEnum? filter, int limit = 100, int offset = 0 )
        {
            var result = await clientConnector.GetChannelParticipants(channelId, channelHash, filter, limit, offset);

            if (result != null)
            {
                return this.Ok(result);
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("channels/invite")]
        public async Task<IHttpActionResult> AddUser([FromBody] ChannelInviteUserRequest request)
        {
            var result = await clientConnector.AddChannelUser(request.ChannelId, request.ChannelHash, request.UserId, request.UserHash);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpPost]
        [AuthorizeKey]
        [Route("channels/remove")]
        public async Task<IHttpActionResult> RemoveUser([FromBody] ChannelRemoveUserRequest request)
        {
            var result = await clientConnector.RemoveChannelUser(request.ChannelId, request.ChannelHash, request.UserId, request.UserHash, request.Kicked);

            if (result)
            {
                return this.Ok();
            }
            return this.InternalServerError();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("channels/{channelId}/history")]
        public async Task<IHttpActionResult> GetHistory(int channelId, long hash, int limit = 0, int offset = 0, int maxId = -1)
        {
            var result = await clientConnector.GetHistory(channelId, hash, limit, offset, maxId, Connector.HistoryPeer.Channel);

            if (result != null)
            {
                return this.Ok(result);
            }
            return this.InternalServerError();
        }
    }
}