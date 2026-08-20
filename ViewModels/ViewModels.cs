using System.ComponentModel.DataAnnotations;
using HamperMarket.Models;

namespace HamperMarket.ViewModels
{
    public class ShopViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public List<string> Labels { get; set; } = new();
        public Dictionary<string, string> SellerNames { get; set; } = new();
        public string? Query { get; set; }
        public string? SelectedCategory { get; set; }
        public string? SelectedLabel { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public User Seller { get; set; } = null!;
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
        public string? Error { get; set; }
    }

    public class SellerRegisterViewModel
    {
        [Required, Display(Name = "Business / Hamper Brand Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "About your hampers")]
        public string? Bio { get; set; }

        [Display(Name = "Shop icon (emoji)")]
        public string LogoEmoji { get; set; } = "🎁";
    }

    public class ProductFormViewModel
    {
        public string? Id { get; set; }

        [Required, Display(Name = "Product / Hamper Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, Range(1, 100000)]
        public decimal Price { get; set; }

        [Required, Range(0, 100000)]
        public int Stock { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Labels (comma separated, e.g. Vegan, Gift for Her, Christmas)")]
        public string LabelsCsv { get; set; } = string.Empty;

        [Display(Name = "Icon (emoji)")]
        public string ImageEmoji { get; set; } = "🎁";
    }

    public class CartLineViewModel
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal LineTotal => Product.Price * Quantity;
    }

    public class CartViewModel
    {
        public List<CartLineViewModel> Lines { get; set; } = new();
        public decimal Subtotal => Lines.Sum(l => l.LineTotal);
    }

    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new();

        [Required, Display(Name = "Full Name")]
        public string BuyerName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string BuyerEmail { get; set; } = string.Empty;

        [Required, Phone]
        public string BuyerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose a shipping option.")]
        public string ShippingCarrier { get; set; } = string.Empty; // "Paxi" or "Pep"

        [Required(ErrorMessage = "Please choose a service level.")]
        public string ShippingServiceName { get; set; } = string.Empty;

        [Display(Name = "PEP / PAXI collection point")]
        public string? ShippingPointName { get; set; }

        [Display(Name = "Delivery address (for PEP Door-to-Door)")]
        public string? DeliveryAddress { get; set; }

        public List<ShippingOption> AvailableOptions { get; set; } = new();
        public List<ShippingPoint> AvailablePoints { get; set; } = new();
    }

    public class PaymentViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Total { get; set; }

        [Required, Display(Name = "Cardholder Name")]
        public string CardHolder { get; set; } = string.Empty;

        [Required, Display(Name = "Card Number"), StringLength(19, MinimumLength = 12)]
        public string CardNumber { get; set; } = string.Empty;

        [Required, Display(Name = "Expiry (MM/YY)")]
        public string Expiry { get; set; } = string.Empty;

        [Required, StringLength(4, MinimumLength = 3)]
        public string Cvv { get; set; } = string.Empty;
    }

    public class AdminDashboardViewModel
    {
        public int TotalSellers { get; set; }
        public int PendingSellers { get; set; }
        public int TotalProducts { get; set; }
        public int PendingProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
    }

    public class SellerDashboardViewModel
    {
        public User Seller { get; set; } = null!;
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
    }
}
