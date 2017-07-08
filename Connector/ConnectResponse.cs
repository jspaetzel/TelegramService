namespace TelegramService.Connector
{
    public class ConnectResponse
    {
        public ConnectorStatus Status { get; set; }

        public string AuthCode { get; set; }
    }
}