using HamperMarket.Models;

namespace HamperMarket.Services
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// A fake payment gateway for demo purposes only. No real card processing occurs.
    /// Use card number 4000 0000 0000 0002 to simulate a declined payment.
    /// </summary>
    public class PaymentService
    {
        public PaymentResult ProcessCardPayment(string cardNumber, string cardHolder, string expiry, string cvv, decimal amount)
        {
            var cleanCard = (cardNumber ?? string.Empty).Replace(" ", "");

            if (cleanCard == "4000000000000002")
            {
                return new PaymentResult
                {
                    Success = false,
                    Reference = string.Empty,
                    Message = "Payment declined by demo bank (insufficient funds). Try a different card number."
                };
            }

            if (cleanCard.Length < 12)
            {
                return new PaymentResult
                {
                    Success = false,
                    Reference = string.Empty,
                    Message = "Invalid card number."
                };
            }

            var reference = "PAY-" + Guid.NewGuid().ToString("N")[..10].ToUpper();
            return new PaymentResult
            {
                Success = true,
                Reference = reference,
                Message = $"Payment of R{amount:N2} approved."
            };
        }
    }
}
