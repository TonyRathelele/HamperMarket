namespace HamperMarket.Models
{
    public enum ProductStatus
    {
        PendingApproval,
        Approved,
        Rejected,
        Hidden
    }

    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string SellerId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public string Category { get; set; } = "General";

        // Labels / tags the seller assigns to the hamper (e.g. "Vegan", "Gift for Her", "Christmas")
        public List<string> Labels { get; set; } = new();

        // Emoji used as a lightweight "image" placeholder for the demo
        public string ImageEmoji { get; set; } = "🎁";

        public ProductStatus Status { get; set; } = ProductStatus.PendingApproval;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
