using System;
using System.Threading.Tasks;
using System.Web.Http;
using TelegramService.Connector;
using TelegramService.Filters;
using TelegramService.Models;
using TeleSharp.TL;

namespace TelegramService.Controllers
{
    public class ConfigController : ApiController
    {

        private readonly TgConnector clientConnector;

        public ConfigController()
        {
            this.clientConnector = new TgConnector();
        }

        /// <summary>
        /// Get the current status of the login of the bot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("config/status")]
        public async Task<string> GetStatus()
        {
            return Enum.GetName(typeof(ConnectorStatus), await clientConnector.GetStatus());
        }

        /// <summary>
        /// Returns a hashcode used by the confirmation in combination with a verification code
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeKey]
        [Route("config/signin")]
        public async Task<ConnectResponse> Get()
        {
            return await clientConnector.Connect();
        }

        /// <summary>
        /// Completes the verification of the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeKey]
        [Route("config/verify")]
        public async Task<TLUser> Verify([FromBody] VerifyRequest request)
        {
            return await clientConnector.Authorize(request.VerifyCode, request.HashCode);
        }
    }
}
