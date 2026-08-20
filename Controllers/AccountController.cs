using System.Security.Claims;
using HamperMarket.Models;
using HamperMarket.Services;
using HamperMarket.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HamperMarket.Controllers;

public class AccountController : Controller
{
    private readonly UserService _users;
    private readonly NotificationService _notify;

    public AccountController(UserService users, NotificationService notify)
    {
        _users = users;
        _notify = notify;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = _users.ValidateLogin(model.Email, model.Password);
        if (user == null)
        {
            model.Error = "Invalid email or password.";
            return View(model);
        }

        if (user.Role == UserRole.Seller && user.Status == SellerStatus.PendingApproval)
        {
            model.Error = "Your seller account is still awaiting admin approval.";
            return View(model);
        }

        if (user.Role == UserRole.Seller && user.Status == SellerStatus.Suspended)
        {
            model.Error = "Your seller account has been suspended. Contact the marketplace admin.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return user.Role == UserRole.Admin ? RedirectToAction("Dashboard", "Admin") : RedirectToAction("Dashboard", "Seller");
    }

    [HttpGet]
    public IActionResult RegisterSeller() => View(new SellerRegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RegisterSeller(SellerRegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (_users.EmailExists(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        var seller = _users.RegisterSeller(model.BusinessName, model.Email, model.Password, model.Phone, model.Bio, model.LogoEmoji);

        _notify.SendEmail(seller.Email, "Welcome to HamperMarket - Application received",
            $"Hi {seller.DisplayName},\n\nThanks for applying to sell on HamperMarket! Your seller account is pending review by our admin team. We'll notify you by email as soon as you're approved.\n\n- The HamperMarket Team",
            null);

        return RedirectToAction("RegisterSuccess");
    }

    [HttpGet]
    public IActionResult RegisterSuccess() => View();

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
