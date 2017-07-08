using System.IO;
using TLSharp.Core;

namespace TelegramService.Connector
{
    public class RelativeFileSessionStore : ISessionStore
    {
        private string binpath;

        public RelativeFileSessionStore()
        {
            //var uriPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            binpath = System.AppDomain.CurrentDomain.BaseDirectory + "/bin";
        }

        public void Save(Session session)
        {
			var path = binpath + "/" + (object) session.SessionUserId + ".dat";
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
				
            {
                byte[] bytes = session.ToBytes();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public Session Load(string sessionUserId)
        {
			string path = binpath + "/" + (object)sessionUserId + ".dat";
            if (!File.Exists(path))
                return (Session)null;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                byte[] buffer = new byte[2048];
                fileStream.Read(buffer, 0, 2048);
                return Session.FromBytes(buffer, (ISessionStore)this, sessionUserId);
            }
        }
    }
}
