using App_MicroService_JobScheduler.Models.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace App_MicroService_JobScheduler.Controllers;

public class AuthController : Controller
{
    private readonly IApplicationDbContext _dbContext;

    public AuthController(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Login(string returnUrl) => View(new LoginModel() { Email = string.Empty, Password = string.Empty, ReturnUrl = returnUrl });

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var adminUser = await _dbContext.AllowedAdmins.SingleOrDefaultAsync(p => p.Email.ToLower() == model.Email.ToLower());

        if (adminUser is null)
        {
            ModelState.AddModelError("Error", "Invalid Credential");
            return View(model);
        }

        var inputPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(model.Password));
        var isPasswordValid = string.Equals(adminUser.Password, inputPassword, StringComparison.OrdinalIgnoreCase);
        if (!isPasswordValid)
        {
            ModelState.AddModelError("Error", "Invalid Credential2");
            return View(model);
        }

        var claims = new List<Claim>()
        {
            new Claim("Email", adminUser.Email),
            new Claim("FirstName", adminUser.FirstName),
            new Claim("LastName", adminUser.LastName)
        };

        var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProps = new AuthenticationProperties()
        {
            AllowRefresh = true,
            IsPersistent = true,
            ExpiresUtc = DateTime.Now.AddDays(1),
            RedirectUri = model.ReturnUrl
        };
        var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
        await HttpContext.SignInAsync(claimsPrincipal, authProps);


        HttpContext.Response.Headers.Add("HX-Redirect", model.ReturnUrl ?? "/");
        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}



public class LoginModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? ReturnUrl { get; set; }
}