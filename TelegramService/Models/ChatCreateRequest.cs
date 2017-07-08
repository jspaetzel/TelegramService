namespace TelegramService.Models
{
    public class ChatCreateRequest
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public long UserHash { get; set; }
    }
}