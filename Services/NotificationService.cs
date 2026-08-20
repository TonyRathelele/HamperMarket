using HamperMarket.Data;
using HamperMarket.Models;

namespace HamperMarket.Services
{
    public class NotificationService
    {
        private readonly JsonStore<MessageLog> _store;

        public NotificationService(JsonStore<MessageLog> store)
        {
            _store = store;
        }

        public MessageLog SendEmail(string to, string subject, string body, string? orderId = null)
        {
            var msg = new MessageLog
            {
                Channel = MessageChannel.Email,
                To = to,
                Subject = subject,
                Body = body,
                RelatedOrderId = orderId
            };
            _store.Add(msg);
            return msg;
        }

        public MessageLog SendSms(string to, string body, string? orderId = null)
        {
            var msg = new MessageLog
            {
                Channel = MessageChannel.Sms,
                To = to,
                Subject = "SMS",
                Body = body,
                RelatedOrderId = orderId
            };
            _store.Add(msg);
            return msg;
        }

        public List<MessageLog> GetAll() => _store.GetAll().OrderByDescending(m => m.SentAt).ToList();

        public List<MessageLog> GetForRecipient(string emailOrPhone) =>
            GetAll().Where(m => m.To.Equals(emailOrPhone, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<MessageLog> GetForOrder(string orderId) =>
            GetAll().Where(m => m.RelatedOrderId == orderId).ToList();
    }
}
