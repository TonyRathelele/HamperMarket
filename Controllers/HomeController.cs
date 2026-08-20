using System.Diagnostics;
using HamperMarket.Models;
using HamperMarket.Services;
using HamperMarket.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HamperMarket.Controllers;

public class HomeController : Controller
{
    private readonly ProductService _products;
    private readonly UserService _users;

    public HomeController(ProductService products, UserService users)
    {
        _products = products;
        _users = users;
    }

    public IActionResult Index()
    {
        var featured = _products.GetApproved().Take(8).ToList();
        ViewBag.Categories = _products.GetCategories();
        return View(featured);
    }

    public IActionResult Shop(string? q, string? category, string? label)
    {
        var vm = new ShopViewModel
        {
            Products = _products.Search(q, category, label),
            Categories = _products.GetCategories(),
            Labels = _products.GetLabels(),
            Query = q,
            SelectedCategory = category,
            SelectedLabel = label
        };
        foreach (var p in vm.Products)
        {
            var seller = _users.GetById(p.SellerId);
            vm.SellerNames[p.Id] = seller?.DisplayName ?? "Marketplace Seller";
        }
        return View(vm);
    }

    public IActionResult ProductDetails(string id)
    {
        var product = _products.GetById(id);
        if (product == null || product.Status != ProductStatus.Approved) return NotFound();
        var seller = _users.GetById(product.SellerId);
        if (seller == null) return NotFound();
        return View(new ProductDetailsViewModel { Product = product, Seller = seller });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
