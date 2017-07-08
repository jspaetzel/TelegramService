using System;

namespace TelegramService.Connector
{
    public class LogMessageEventArgs : EventArgs
    {
		public string Level { get; set; }
		public string Message { get; set; }

        public LogMessageEventArgs(string level, string message)
        {
            Level = level;
            Message = message;
        }
    }
}