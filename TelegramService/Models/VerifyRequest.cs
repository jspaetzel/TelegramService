namespace TelegramService.Models
{
    public class VerifyRequest
    {
        public string HashCode { get; set; }
        public int VerifyCode { get; set; }
    }
}