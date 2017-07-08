namespace TelegramService.Models
{
    public class ChannelInviteUserRequest
    {
        public int UserId;

        public long UserHash;

        public int ChannelId;

        public long ChannelHash;
    }
}