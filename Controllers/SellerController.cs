using System.Security.Claims;
using HamperMarket.Models;
using HamperMarket.Services;
using HamperMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HamperMarket.Controllers;

[Authorize(Roles = "Seller")]
public class SellerController : Controller
{
    private readonly ProductService _products;
    private readonly OrderService _orders;
    private readonly UserService _users;

    public SellerController(ProductService products, OrderService orders, UserService users)
    {
        _products = products;
        _orders = orders;
        _users = users;
    }

    private string CurrentSellerId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public IActionResult Dashboard()
    {
        var seller = _users.GetById(CurrentSellerId)!;
        var myProducts = _products.GetBySeller(CurrentSellerId);
        var myOrders = _orders.GetForSeller(CurrentSellerId);

        var vm = new SellerDashboardViewModel
        {
            Seller = seller,
            TotalProducts = myProducts.Count,
            TotalOrders = myOrders.Count,
            TotalRevenue = myOrders.Where(o => o.PaymentStatus == PaymentStatus.Approved)
                .SelectMany(o => o.Items.Where(i => i.SellerId == CurrentSellerId))
                .Sum(i => i.LineTotal),
            RecentOrders = myOrders.Take(5).ToList()
        };
        return View(vm);
    }

    public IActionResult Products()
    {
        return View(_products.GetBySeller(CurrentSellerId));
    }

    [HttpGet]
    public IActionResult CreateProduct() => View("ProductForm", new ProductFormViewModel());

    [HttpGet]
    public IActionResult EditProduct(string id)
    {
        var product = _products.GetById(id);
        if (product == null || product.SellerId != CurrentSellerId) return NotFound();

        var vm = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            LabelsCsv = string.Join(", ", product.Labels),
            ImageEmoji = product.ImageEmoji
        };
        return View("ProductForm", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveProduct(ProductFormViewModel model)
    {
        if (!ModelState.IsValid) return View("ProductForm", model);

        var labels = (model.LabelsCsv ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();

        if (string.IsNullOrEmpty(model.Id))
        {
            var product = new Product
            {
                SellerId = CurrentSellerId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Category = model.Category,
                Labels = labels,
                ImageEmoji = string.IsNullOrWhiteSpace(model.ImageEmoji) ? "🎁" : model.ImageEmoji,
                Status = ProductStatus.PendingApproval
            };
            _products.Create(product);
            TempData["Toast"] = "Product submitted! It will appear in the shop once approved by an admin.";
        }
        else
        {
            var existing = _products.GetById(model.Id);
            if (existing == null || existing.SellerId != CurrentSellerId) return NotFound();

            _products.Update(model.Id, p =>
            {
                p.Name = model.Name;
                p.Description = model.Description;
                p.Price = model.Price;
                p.Stock = model.Stock;
                p.Category = model.Category;
                p.Labels = labels;
                p.ImageEmoji = string.IsNullOrWhiteSpace(model.ImageEmoji) ? "🎁" : model.ImageEmoji;
                // Edits go back to pending review to keep the catalogue moderated
                p.Status = ProductStatus.PendingApproval;
            });
            TempData["Toast"] = "Product updated and resubmitted for admin approval.";
        }

        return RedirectToAction("Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProduct(string id)
    {
        var existing = _products.GetById(id);
        if (existing == null || existing.SellerId != CurrentSellerId) return NotFound();
        _products.Delete(id);
        TempData["Toast"] = "Product removed.";
        return RedirectToAction("Products");
    }

    public IActionResult Orders()
    {
        ViewBag.CurrentSellerId = CurrentSellerId;
        return View(_orders.GetForSeller(CurrentSellerId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderStatus(string orderId, OrderStatus status)
    {
        var order = _orders.GetById(orderId);
        if (order == null || !order.Items.Any(i => i.SellerId == CurrentSellerId)) return NotFound();
        _orders.SetStatus(orderId, status);
        TempData["Toast"] = $"Order {orderId} marked as {status}.";
        return RedirectToAction("Orders");
    }

    public IActionResult Profile() => View(_users.GetById(CurrentSellerId));
}
