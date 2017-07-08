using System.Threading.Tasks;
using System.Web.Http;
using TelegramService.Connector;
using TelegramService.Filters;
using TeleSharp.TL;
using TeleSharp.TL.Messages;

namespace TelegramService.Controllers
{
    public class UsersController : ApiController
    {
        private readonly TgConnector clientConnector;

        public UsersController()
        {
            this.clientConnector = new TgConnector();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("users/contacts")]
        public async Task<TLVector<TLAbsUser>> GetContacts()
        {
            return await clientConnector.GetUserContacts();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("users/chats")]
        public async Task<TLChats> GetChats()
        {
            return await clientConnector.GetChats();
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("users")]
        public async Task<TLAbsUser> GetUser(string userName)
        {
            return await clientConnector.ResolveUsername(userName);
        }

        [HttpGet]
        [AuthorizeKey]
        [Route("users/{userId}/history")]
        public async Task<IHttpActionResult> GetHistory(int userId, long hash, int limit = 0, int offset = 0, int maxId = -1)
        {
            var result = await clientConnector.GetHistory(userId, hash, limit, offset, maxId, Connector.HistoryPeer.User);

            if (result != null)
            {
                return this.Ok(result);
            }
            return this.InternalServerError();
        }
    }
}