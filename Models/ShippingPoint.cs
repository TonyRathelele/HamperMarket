namespace HamperMarket.Models
{
    public class ShippingPoint
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class ShippingOption
    {
        public ShippingCarrier Carrier { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EtaText { get; set; } = string.Empty;
        public decimal Cost { get; set; }
    }

    public class CartItem
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
