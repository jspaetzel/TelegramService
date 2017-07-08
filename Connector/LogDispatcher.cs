using System;

namespace TelegramService.Connector
{
    public class LogDispatcher
    {
        public static event EventHandler<LogMessageEventArgs> MessageLogged;

        public static void Dispatch(string level, string message)
        {
			if (MessageLogged != null)
			{
				MessageLogged.Invoke(null, new LogMessageEventArgs(level, message));
			}
        }
    }
}
