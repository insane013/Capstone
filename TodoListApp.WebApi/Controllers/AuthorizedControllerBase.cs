using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Base controller class for Capstone API controllers.
/// To get current user Id use [RequireUserId] attribute with UserID property.
/// API bad response should be in fomat Message = "response".
/// </summary>
public abstract class AuthorizedControllerBase : ControllerBase
{
    protected AuthorizedControllerBase(ILogger logger)
    {
        this.Logger = logger;
    }

    /// <summary>
    /// Gets Logger instanse.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets currently authorized UserId. You must use it only in methods with attribute [RequireUserId] or it returns null.
    /// </summary>
    protected string? UserId
    {
        get => this.HttpContext?.Items["UserId"]?.ToString();
    }

    /// <summary>
    /// Gets Id of currently authorized user.
    /// </summary>
    /// <returns>User ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Throws if there is no authorized users.</exception>
    protected string GetUserIdOrThrow()
    {
        var userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            LoggingDelegates.LogWarn(this.Logger, $"Unauthorized user access.", null);
            throw new UnauthorizedAccessException("Unauthorized user access.");
        }

        return userId;
    }

    /// <summary>
    /// Checks wheither model is valid and make log if not.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    /// <param name="model">Model to validate.</param>
    /// <returns>True or false.</returns>
    protected bool ValidateModel<T>(T model)
    {
        if (!this.ModelState.IsValid || model is null)
        {
            LoggingDelegates.LogIncorrectData(this.Logger, model?.ToString() ?? "null", null);
            return false;
        }

        return true;
    }
}
