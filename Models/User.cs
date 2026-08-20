namespace HamperMarket.Models
{
    public enum UserRole
    {
        Admin,
        Seller
    }

    public enum SellerStatus
    {
        PendingApproval,
        Approved,
        Suspended
    }

    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public UserRole Role { get; set; }

        // Login
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;

        // Profile
        public string DisplayName { get; set; } = string.Empty; // Business name for sellers
        public string Phone { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? LogoEmoji { get; set; } = "🎁";

        // Seller-specific
        public SellerStatus Status { get; set; } = SellerStatus.PendingApproval;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
