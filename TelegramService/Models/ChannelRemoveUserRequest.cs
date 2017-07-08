namespace TelegramService.Models
{
    public class ChannelRemoveUserRequest
    {
        public int UserId;

        public long UserHash;

        public int ChannelId;

        public long ChannelHash;

        public bool Kicked;
    }
}