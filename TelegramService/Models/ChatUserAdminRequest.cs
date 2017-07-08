namespace TelegramService.Models
{
    public class ChatUserAdminRequest
    {
        public int UserId { get; set; }
        public long UserHash { get; set; }
        public bool IsAdmin { get; set; }
    }
}