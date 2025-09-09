using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models.User;
using TodoListApp.Models.User.Authorization;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Controllers;

[Route("Account")]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAuthorizationWebApiService authService;
    private readonly ILogger<AccountController> logger;

    public AccountController(IAuthorizationWebApiService authService, ILogger<AccountController> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    [HttpGet]
    [Route("Login")]
    public IActionResult Login()
    {
        return this.View();
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginUserModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            string token = await this.authService.LoginToApi(model);

            this.Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(1),
            });

            var claimsPrincipal = this.authService.GetPrincipalFromToken(token);

            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            this.logger.LogWarning($"Login success: \n{token}");

            return this.RedirectToAction("AllTodoLists", "TodoList");
        }
        catch (ApplicationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
            return this.View(model);
        }
    }

    public async Task<IActionResult> Register(RegisterUserModel model)
    {
        if (!this.ModelState.IsValid)
        {
            this.TempData["ErrorMessage"] = "Not valid registration data.";
            return this.View(model);
        }

        var result = await this.authService.Register(model);

        if (result)
        {
            this.TempData["Success"] = "Registration complete";
            return this.RedirectToAction("Login");
        }

        this.TempData["ErrorMessage"] = "Registration failed";
        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        this.Response.Cookies.Delete("access_token");

        return this.RedirectToAction("Login", "Account");
    }
}
