using HamperMarket.Models;
using HamperMarket.Services;
using HamperMarket.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HamperMarket.Controllers;

public class CheckoutController : Controller
{
    private readonly CartService _cart;
    private readonly ProductService _products;
    private readonly UserService _users;
    private readonly ShippingService _shipping;
    private readonly PaymentService _payment;
    private readonly OrderService _orders;
    private readonly NotificationService _notify;

    public CheckoutController(CartService cart, ProductService products, UserService users,
        ShippingService shipping, PaymentService payment, OrderService orders, NotificationService notify)
    {
        _cart = cart;
        _products = products;
        _users = users;
        _shipping = shipping;
        _payment = payment;
        _orders = orders;
        _notify = notify;
    }

    private CartViewModel BuildCart()
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

    [HttpGet]
    public IActionResult Index()
    {
        var cart = BuildCart();
        if (!cart.Lines.Any()) return RedirectToAction("Index", "Cart");

        var vm = new CheckoutViewModel
        {
            Cart = cart,
            AvailableOptions = _shipping.GetOptions(),
            AvailablePoints = _shipping.GetPickupPoints()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PlaceOrder(CheckoutViewModel model)
    {
        var cart = BuildCart();
        if (!cart.Lines.Any()) return RedirectToAction("Index", "Cart");

        model.Cart = cart;
        model.AvailableOptions = _shipping.GetOptions();
        model.AvailablePoints = _shipping.GetPickupPoints();

        if (!Enum.TryParse<ShippingCarrier>(model.ShippingCarrier, out var carrier))
        {
            ModelState.AddModelError(nameof(model.ShippingCarrier), "Please choose a shipping carrier.");
        }

        var option = _shipping.GetOption(carrier, model.ShippingServiceName);
        if (option == null)
        {
            ModelState.AddModelError(nameof(model.ShippingServiceName), "Please choose a valid shipping service.");
        }

        var needsPoint = carrier == ShippingCarrier.Paxi;
        if (needsPoint && string.IsNullOrWhiteSpace(model.ShippingPointName))
        {
            ModelState.AddModelError(nameof(model.ShippingPointName), "Please choose a PEP/PAXI collection point.");
        }
        if (!needsPoint && string.IsNullOrWhiteSpace(model.DeliveryAddress))
        {
            ModelState.AddModelError(nameof(model.DeliveryAddress), "Please provide a delivery address for door-to-door courier.");
        }

        if (!ModelState.IsValid) return View("Index", model);

        var order = new Order
        {
            BuyerName = model.BuyerName,
            BuyerEmail = model.BuyerEmail,
            BuyerPhone = model.BuyerPhone,
            Carrier = carrier,
            ShippingServiceName = option!.ServiceName,
            ShippingCost = option.Cost,
            Status = OrderStatus.PendingPayment
        };

        if (needsPoint)
        {
            var point = _shipping.GetPickupPoints().FirstOrDefault(p => p.Name == model.ShippingPointName);
            order.ShippingPointName = point?.Name ?? model.ShippingPointName ?? "";
            order.ShippingPointAddress = point?.Address ?? "";
        }
        else
        {
            order.ShippingPointName = "Home / Office delivery";
            order.ShippingPointAddress = model.DeliveryAddress ?? "";
        }

        foreach (var line in cart.Lines)
        {
            var seller = _users.GetById(line.Product.SellerId);
            order.Items.Add(new OrderItem
            {
                ProductId = line.Product.Id,
                ProductName = line.Product.Name,
                SellerId = line.Product.SellerId,
                SellerName = seller?.DisplayName ?? "Seller",
                UnitPrice = line.Product.Price,
                Quantity = line.Quantity,
                ImageEmoji = line.Product.ImageEmoji
            });
        }

        _orders.Create(order);

        // Stash order id for the payment step
        HttpContext.Session.SetString("PendingOrderId", order.Id);

        return RedirectToAction("Payment", new { orderId = order.Id });
    }

    [HttpGet]
    public IActionResult Payment(string orderId)
    {
        var order = _orders.GetById(orderId);
        if (order == null) return NotFound();
        if (order.PaymentStatus == PaymentStatus.Approved) return RedirectToAction("Confirmation", new { orderId });

        return View(new PaymentViewModel { OrderId = order.Id, Total = order.Total });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessPayment(PaymentViewModel model)
    {
        var order = _orders.GetById(model.OrderId);
        if (order == null) return NotFound();
        model.Total = order.Total;

        if (!ModelState.IsValid) return View("Payment", model);

        var result = _payment.ProcessCardPayment(model.CardNumber, model.CardHolder, model.Expiry, model.Cvv, order.Total);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View("Payment", model);
        }

        var trackingNumber = _shipping.GenerateTrackingNumber(order.Carrier);

        _orders.Update(order.Id, o =>
        {
            o.PaymentStatus = PaymentStatus.Approved;
            o.PaymentReference = result.Reference;
            o.Status = OrderStatus.Paid;
            o.TrackingNumber = trackingNumber;
        });

        foreach (var item in order.Items)
        {
            _products.DecrementStock(item.ProductId, item.Quantity);
        }

        // Simulated notifications
        var pointInfo = order.Carrier == ShippingCarrier.Paxi
            ? $"Collect from: {order.ShippingPointName}, {order.ShippingPointAddress}"
            : $"Delivering to: {order.ShippingPointAddress}";

        _notify.SendEmail(order.BuyerEmail, $"Order {order.Id} confirmed - HamperMarket",
            $"Hi {order.BuyerName},\n\nThank you for your order! We've received payment of R{order.Total:N2} (ref {result.Reference}).\n\n" +
            $"Shipping: {order.ShippingServiceName} via {order.Carrier}\n{pointInfo}\nTracking number: {trackingNumber}\n\n" +
            $"We'll let you know the moment it ships.\n\n- The HamperMarket Team", order.Id);

        _notify.SendSms(order.BuyerPhone,
            $"HamperMarket: Order {order.Id} confirmed! R{order.Total:N2} paid. Tracking: {trackingNumber}. Thank you for shopping with us!", order.Id);

        foreach (var sellerId in order.Items.Select(i => i.SellerId).Distinct())
        {
            var seller = _users.GetById(sellerId);
            if (seller == null) continue;
            var itemsForSeller = order.Items.Where(i => i.SellerId == sellerId).ToList();
            var itemsList = string.Join("\n", itemsForSeller.Select(i => $"  - {i.Quantity} x {i.ProductName} (R{i.LineTotal:N2})"));
            _notify.SendEmail(seller.Email, $"New order {order.Id} - HamperMarket",
                $"Hi {seller.DisplayName},\n\nYou have a new paid order!\n\n{itemsList}\n\nShip via: {order.ShippingServiceName}\n{pointInfo}\n\nPlease prepare this order for dispatch.\n\n- HamperMarket", order.Id);
        }

        _cart.Clear();

        return RedirectToAction("Confirmation", new { orderId = order.Id });
    }

    [HttpGet]
    public IActionResult Confirmation(string orderId)
    {
        var order = _orders.GetById(orderId);
        if (order == null) return NotFound();
        return View(order);
    }
}
