namespace TelegramService.Models
{
    public class ChatAddUserRequest
    {
        public int UserId;
        public long UserHash;
        public int ForwardMessageCount = 0;
    }
}