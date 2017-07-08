using System.IO;
using System.Xml.Serialization;
using TLSharp.Core;

namespace TelegramService.Connector
{
    class SerializedSingleSessionStore : ISessionStore
    {
        readonly string sessionUserId;
        
        readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Session));

        public string SessionData { get; private set; }

        public SerializedSingleSessionStore(string sessionUserId, string sessionData)
        {
            this.sessionUserId = sessionUserId;
            SessionData = sessionData;
        }

        public void Save(Session session)
        {
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, session);
                SessionData = stringWriter.ToString();
            }
        }

        public Session Load(string sessionUserId)
        {
            if (sessionUserId != this.sessionUserId) return null;

            using(var stringReader = new StringReader(SessionData))
                return xmlSerializer.Deserialize(stringReader) as Session;
        }
    }
}