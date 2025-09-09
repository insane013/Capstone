using System.Security.Claims;
using TodoListApp.Models.User;
using TodoListApp.Models.User.Authorization;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface IAuthorizationWebApiService
{
    public Task<string> LoginToApi(LoginUserModel model);

    public Task<bool> Register(RegisterUserModel model);

    public ClaimsPrincipal? GetPrincipalFromToken(string token);
}
