using HamperMarket.Services;
using HamperMarket.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HamperMarket.Controllers;

public class CartController : Controller
{
    private readonly CartService _cart;
    private readonly ProductService _products;

    public CartController(CartService cart, ProductService products)
    {
        _cart = cart;
        _products = products;
    }

    private CartViewModel BuildViewModel()
    {
        var vm = new CartViewModel();
        foreach (var item in _cart.GetItems())
        {
            var product = _products.GetById(item.ProductId);
            if (product == null) continue;
            vm.Lines.Add(new CartLineViewModel { Product = product, Quantity = item.Quantity });
        }
        return vm;
    }

    public IActionResult Index() => View(BuildViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(string productId, int quantity = 1)
    {
        var product = _products.GetById(productId);
        if (product == null) return NotFound();
        if (quantity < 1) quantity = 1;
        _cart.AddItem(productId, quantity);
        TempData["Toast"] = $"Added \"{product.Name}\" to your cart.";

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer)) return Redirect(referer);
        return RedirectToAction("Shop", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(string productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(string productId)
    {
        _cart.RemoveItem(productId);
        return RedirectToAction("Index");
    }
}
