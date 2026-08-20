namespace HamperMarket.Models
{
    public enum MessageChannel
    {
        Email,
        Sms
    }

    public class MessageLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public MessageChannel Channel { get; set; }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty; // used as headline for SMS too
        public string Body { get; set; } = string.Empty;
        public string? RelatedOrderId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
