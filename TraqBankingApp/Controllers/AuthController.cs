using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TraqBankingApp.Data;
using TraqBankingApp.Models;

namespace TraqBankingApp.Controllers;

public class AuthController : Controller
{
    private readonly PeopleManagerContext _db;
    private readonly PasswordHasher<string> _hasher = new();

    public AuthController(PeopleManagerContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(ViewModels.LoginInput input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = await _db.UserLogins.SingleOrDefaultAsync(u => u.Username == input.Username);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials.");
            return View(input);
        }

        var result = _hasher.VerifyHashedPassword(user.Username, user.PasswordHash, input.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid credentials.");
            return View(input);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // Sign up
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Signup(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Signup(ViewModels.SignUpInput input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(input);

        var exists = await _db.UserLogins.AnyAsync(u => u.Username == input.Username);
        if (exists)
        {
            ModelState.AddModelError("Username", "Username already exists.");
            return View(input);
        }

        var hash = _hasher.HashPassword(input.Username, input.Password);
        var user = new UserLogin
        {
            Username = input.Username,
            PasswordHash = hash
        };

        _db.UserLogins.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Username", "Username already exists.");
            return View(input);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}