using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoListApp.WebApi.Helpers.Attributes;

/// <summary>
/// Use this attributes on methods, where you need to get UserId of currently authorized user.
/// You can get Id from HttpContext.Items["UserId"] or use AuthorizedControllerBase class and its property UserId.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class RequireUserIdAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Gets UserId of currently authorized user and writes to context.
    /// </summary>
    /// <param name="context">Current HttpContext.</param>
    /// <param name="next">Method, which calls this attribute.</param>
    /// <returns>None.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown If there is no authorized users.</exception>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User ID claim not found");
        }

        context.HttpContext.Items["UserId"] = userId;

        _ = await next();
    }
}
