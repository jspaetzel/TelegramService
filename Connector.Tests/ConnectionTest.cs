using System.Web.Configuration;
using NUnit.Framework;
using TLSharp.Core;

namespace Connector.Tests
{
    [TestFixture]
    public class ConnectionTest
    {
        [Test]
        public void SuccessfulConnection()
        {
            var apiId = int.Parse(WebConfigurationManager.AppSettings["ApiId"]);
            var apiHash = WebConfigurationManager.AppSettings["ApiHash"];

            var client = new TelegramClient(apiId, apiHash);
            var result = client.ConnectAsync().Result;
            Assert.True(result);
        }
    }
}
