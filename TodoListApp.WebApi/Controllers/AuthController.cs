using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.Helpers;
using TodoListApp.Models.User;
using TodoListApp.Models.User.Authorization;
using TodoListApp.Services.Database.Users.Identity;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Authorization API endpoint.
/// </summary>
[AllowAnonymous]
[Route("authorization")]
public class AuthController : AuthorizedControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">Identity User manager instance.</param>
    /// <param name="configuration">Configuration access.</param>
    /// <param name="logger">Logger instance.</param>
    public AuthController(UserManager<User> userManager, IConfiguration configuration, ILogger<AuthController> logger)
        : base(logger)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    /// <summary>
    /// Register new user endpoint.
    /// </summary>
    /// <param name="model">RegisterUserModel in json format.</param>
    /// <returns>Success or error code.</returns>
    [HttpPost]
    [Route("Register")]
    [Consumes("application/json")]
    public async Task<IActionResult> Register([FromBody] RegisterUserModel model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = "Incorrect user data." });
        }

        var user = new User
        {
            Email = model.Email,
            UserName = model.Email,
            Name = model.Name ?? model.Email,
            UniqueTag = model.UniqueTag,
            RegistrationTime = DateTime.UtcNow,
        };

        var result = await this.userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return this.Ok(new { Message = "User registered successfully" });
        }

        return this.BadRequest(new { Message = result.Errors });
    }

    /// <summary>
    /// Login endpoint.
    /// </summary>
    /// <param name="model">LoginUserModel in json.</param>
    /// <returns>Jwt token or error code.</returns>
    [HttpPost]
    [Route("Login")]
    [Consumes("application/json")]
    public async Task<IActionResult> Login([FromBody] LoginUserModel model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = "Incorrect user data." });
        }

        LoggingDelegates.LogWarn(this.Logger, "Login attempt.", null);

        var user = await this.userManager.FindByEmailAsync(model.Login);

        if (user == null || !await this.userManager.CheckPasswordAsync(user, model.Password))
        {
            return this.Unauthorized(new { Message = "Invalid login attempt" });
        }

        var token = this.GenerateJwtToken(user);

        return this.Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? "name"),
            new Claim(ClaimTypes.Email, user.Email ?? "email@email.com"),
        };

        string? stringKey = this.configuration["Jwt:Key"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(stringKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this.configuration["Jwt:Issuer"],
            audience: this.configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
