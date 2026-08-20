using HamperMarket.Models;
using HamperMarket.Services;
using HamperMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HamperMarket.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserService _users;
    private readonly ProductService _products;
    private readonly OrderService _orders;
    private readonly NotificationService _notify;

    public AdminController(UserService users, ProductService products, OrderService orders, NotificationService notify)
    {
        _users = users;
        _products = products;
        _orders = orders;
        _notify = notify;
    }

    public IActionResult Dashboard()
    {
        var sellers = _users.GetAllSellers();
        var products = _products.GetAll();
        var orders = _orders.GetAll();

        var vm = new AdminDashboardViewModel
        {
            TotalSellers = sellers.Count,
            PendingSellers = sellers.Count(s => s.Status == SellerStatus.PendingApproval),
            TotalProducts = products.Count,
            PendingProducts = products.Count(p => p.Status == ProductStatus.PendingApproval),
            TotalOrders = orders.Count,
            TotalRevenue = orders.Where(o => o.PaymentStatus == PaymentStatus.Approved).Sum(o => o.Total),
            RecentOrders = orders.Take(5).ToList()
        };
        return View(vm);
    }

    public IActionResult Sellers() => View(_users.GetAllSellers());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApproveSeller(string id)
    {
        var seller = _users.GetById(id);
        if (seller == null) return NotFound();
        _users.SetSellerStatus(id, SellerStatus.Approved);
        _notify.SendEmail(seller.Email, "You're approved! - HamperMarket",
            $"Hi {seller.DisplayName},\n\nGreat news - your seller account has been approved. You can now log in and start listing your hampers.\n\n- The HamperMarket Team");
        TempData["Toast"] = $"{seller.DisplayName} approved.";
        return RedirectToAction("Sellers");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SuspendSeller(string id)
    {
        var seller = _users.GetById(id);
        if (seller == null) return NotFound();
        _users.SetSellerStatus(id, SellerStatus.Suspended);
        _notify.SendEmail(seller.Email, "Account suspended - HamperMarket",
            $"Hi {seller.DisplayName},\n\nYour seller account has been suspended by an administrator. Please contact support for more information.\n\n- The HamperMarket Team");
        TempData["Toast"] = $"{seller.DisplayName} suspended.";
        return RedirectToAction("Sellers");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ReapproveSeller(string id)
    {
        var seller = _users.GetById(id);
        if (seller == null) return NotFound();
        _users.SetSellerStatus(id, SellerStatus.Approved);
        TempData["Toast"] = $"{seller.DisplayName} re-approved.";
        return RedirectToAction("Sellers");
    }

    public IActionResult Products()
    {
        var products = _products.GetAll();
        ViewBag.SellerNames = _users.GetAllSellers().ToDictionary(s => s.Id, s => s.DisplayName);
        return View(products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApproveProduct(string id)
    {
        var product = _products.GetById(id);
        if (product == null) return NotFound();
        _products.SetStatus(id, ProductStatus.Approved);

        var seller = _users.GetById(product.SellerId);
        if (seller != null)
        {
            _notify.SendEmail(seller.Email, $"\"{product.Name}\" approved - HamperMarket",
                $"Hi {seller.DisplayName},\n\nYour product \"{product.Name}\" is now live in the HamperMarket shop.\n\n- The HamperMarket Team");
        }
        TempData["Toast"] = $"\"{product.Name}\" approved.";
        return RedirectToAction("Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RejectProduct(string id)
    {
        var product = _products.GetById(id);
        if (product == null) return NotFound();
        _products.SetStatus(id, ProductStatus.Rejected);

        var seller = _users.GetById(product.SellerId);
        if (seller != null)
        {
            _notify.SendEmail(seller.Email, $"\"{product.Name}\" needs changes - HamperMarket",
                $"Hi {seller.DisplayName},\n\nYour product \"{product.Name}\" was not approved. Please review it and resubmit.\n\n- The HamperMarket Team");
        }
        TempData["Toast"] = $"\"{product.Name}\" rejected.";
        return RedirectToAction("Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProduct(string id)
    {
        _products.Delete(id);
        TempData["Toast"] = "Product deleted.";
        return RedirectToAction("Products");
    }

    public IActionResult Orders() => View(_orders.GetAll());

    public IActionResult Messages() => View(_notify.GetAll());
}
