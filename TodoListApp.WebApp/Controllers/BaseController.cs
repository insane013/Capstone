using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Base WebApp controller.
/// </summary>
public class BaseController : Controller
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    protected BaseController(ILogger logger)
    {
        this.Logger = logger;
    }

    /// <summary>
    /// Gets JWT token from cooky.
    /// </summary>
    protected string? Token => this.Request.Cookies["access_token"];

    /// <summary>
    /// Gets Logger instance.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets Referer Uri.
    /// </summary>
    protected string Referer
    {
        get
        {
            var refer = this.HttpContext.Request.Headers.Referer.ToString();

            if (!string.IsNullOrEmpty(refer))
            {
                return refer;
            }

            return "/";
        }
    }

    /// <summary>
    /// Executes a function in try-catch case. Run all your service interactions using this method.
    /// </summary>
    /// <param name="func">Your code, that can throw ApplicationException.</param>
    /// <param name="catchResult">Return value if exception occured.</param>
    /// <param name="errorMessage">Custom error message showed to user. Contains exception message by default.</param>
    /// <param name="errorLog">Custom error log message. Contains exception message by default.</param>
    /// <returns>Function result value or catchResult.</returns>
    protected async Task<IActionResult> Execute(Func<Task<IActionResult>> func, IActionResult catchResult, string? errorMessage = null, string? errorLog = null)
    {
        ArgumentNullException.ThrowIfNull(func);
        try
        {
            return await func();
        }
        catch (ApplicationException ex)
        {
            this.TempData["ErrorMessage"] = errorMessage ?? ex.Message;
            LoggingDelegates.LogWarn(this.Logger, errorLog ?? ex.Message, ex);
            return catchResult;
        }
    }

    /// <summary>
    /// Redirection to url or to default route if return url is null.
    /// </summary>
    /// <param name="returnUrl">Url to return.</param>
    /// <param name="fallbackAction">Controller action redirect to if returnUrl is null.</param>
    /// <param name="fallbackController">Controller name, if returnUrl is null.</param>
    /// <param name="id">Route ID value if needed.</param>
    /// <returns>Redirection result.</returns>
    protected IActionResult RedirectToReturnUrl(Uri? returnUrl, string fallbackAction, string fallbackController, long? id = null)
    {
        if (returnUrl != null)
        {
            return this.Redirect(returnUrl.ToString());
        }

        return id.HasValue
            ? this.RedirectToAction(fallbackAction, fallbackController, new { id = id.Value })
            : this.RedirectToAction(fallbackAction, fallbackController);
    }

    /// <summary>
    /// Checks wheither model is valid and make log if not.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    /// <param name="model">Model to validate.</param>
    /// <returns>True or false.</returns>
    protected bool ValidateModel<T>(T model, string errorMessage = $"Invalid data.")
    {
        if (!this.ModelState.IsValid || model is null)
        {
            this.TempData["ErrorMessage"] = errorMessage;

            LoggingDelegates.LogIncorrectData(this.Logger, model?.ToString() ?? "null", null);
            return false;
        }

        return true;
    }
}
