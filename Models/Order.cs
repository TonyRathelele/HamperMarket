namespace HamperMarket.Models
{
    public enum OrderStatus
    {
        PendingPayment,
        Paid,
        Preparing,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum PaymentStatus
    {
        Pending,
        Approved,
        Declined
    }

    public enum ShippingCarrier
    {
        Paxi,
        Pep
    }

    public class OrderItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageEmoji { get; set; } = "🎁";

        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class Order
    {
        public string Id { get; set; } = "HM-" + Guid.NewGuid().ToString("N")[..8].ToUpper();

        public List<OrderItem> Items { get; set; } = new();

        // Guest checkout details
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set; } = string.Empty;

        // Shipping
        public ShippingCarrier Carrier { get; set; } = ShippingCarrier.Paxi;
        public string ShippingServiceName { get; set; } = string.Empty; // e.g. "PAXI Standard (5-7 days)"
        public string ShippingPointName { get; set; } = string.Empty;   // Chosen PEP/PAXI store point
        public string ShippingPointAddress { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;

        // Payment
        public string PaymentMethod { get; set; } = "Demo Card";
        public string PaymentReference { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public decimal Total => Subtotal + ShippingCost;

        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
