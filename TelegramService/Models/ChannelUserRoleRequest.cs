using TelegramService.Connector;

namespace TelegramService.Models
{
    public class ChannelUserRoleRequest
    {
        public long ChannelHash;
        public long UserHash;
        public RoleEnum Role;
    }
}