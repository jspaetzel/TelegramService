namespace TelegramService.Models
{
    public class ChannelCreateRequest
    {
        public string Title { get; set; }
        public string About { get; set; }
        public bool MegaGroup { get; set; }
        public bool Broadcast { get; set; }
    }
}