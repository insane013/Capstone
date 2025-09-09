using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.User;
using TodoListApp.Services;
using TodoListApp.WebApi.Helpers.Attributes;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// User info API endpoint.
/// </summary>
[Authorize]
[Route("/Users")]
public class UserController : AuthorizedControllerBase
{
    private readonly IUserDatabaseService userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="dbService">Database Interaction service instace.</param>
    /// <param name="logger">Logger instance.</param>
    public UserController(IUserDatabaseService userService, ILogger<UserController> logger)
        : base(logger)
    {
        this.userService = userService;
    }

    /// <summary>
    /// Get list of users by filter endpoint.
    /// </summary>
    /// <param name="filter">UserFilter from query string.</param>
    /// <returns>PaginatedResult with ViewuserInfo or error code.</returns>
    [HttpGet]
    [RequireUserId]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(filter))
        {
            return this.BadRequest(new { Message = $"Incorrect filter format:\n{filter}" });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching Users with filter:\n{filter}", null);

        var data = await this.userService.GetAllAsync(this.UserId!, filter);

        if (data.Items.Any())
        {
            LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);
            return this.Ok(data);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"No data with this filter:\n{filter}", null);
            return this.NotFound(new { Message = $"No data with this filter:\n{filter}" });
        }
    }

    /// <summary>
    /// Get users by its tag endpoint.
    /// </summary>
    /// <param name="tag">User tag from query string.</param>
    /// <returns>ViewuserInfo or error code.</returns>
    [HttpGet]
    [Route("Tag")]
    public async Task<IActionResult> GetUserByTag([FromQuery] string tag)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(tag))
        {
            return this.BadRequest(new { Message = $"Incorrect tag format:\n{tag}" });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting user info with tag: {tag}", null);

        var data = await this.userService.GetByTagAsync(tag);

        if (data is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);
            return this.Ok(data);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"User with tag #{tag} not found.", null);
            return this.NotFound(new { Message = $"User with tag #{tag} not found." });
        }
    }

    /// <summary>
    /// Get users by its email endpoint.
    /// </summary>
    /// <param name="email">User email from query string.</param>
    /// <returns>ViewuserInfo or error code.</returns>
    [HttpGet]
    [Route("Email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery, EmailAddress] string email)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(email))
        {
            return this.BadRequest(new { Message = $"Email {email} is not valid." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting user info with email: {email}", null);

        var data = await this.userService.GetByEmailAsync(email);

        if (data is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);
            return this.Ok(data);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"User with email #{email} not found.", null);
            return this.NotFound(new { Message = $"User with email #{email} not found." });
        }
    }

    /// <summary>
    /// Get users by its id endpoint.
    /// </summary>
    /// <param name="userId">User id from query string.</param>
    /// <returns>ViewuserInfo or error code.</returns>
    [HttpGet]
    [Route("Id")]
    public async Task<IActionResult> GetUserById([FromQuery] string userId)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(userId))
        {
            return this.BadRequest(new { Message = $"Incorrect userID." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting user info with id: {userId}", null);

        var data = await this.userService.GetByIdAsync(userId);

        if (data is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);
            return this.Ok(data);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"User with id #{userId} not found.", null);
            return this.NotFound(new { Message = $"User with this id not found." });
        }
    }

    /// <summary>
    /// Change user's tag endpoint.
    /// </summary>
    /// <param name="tag">new user tag from query string.</param>
    /// <returns>ViewUserInfo or error code.</returns>
    [HttpPut]
    [RequireUserId]
    [Route("Tag")]
    public async Task<IActionResult> ChangeCurrentUserTag([FromQuery] string tag)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(tag))
        {
            return this.BadRequest(new { Message = $"Incorrect tag format: {tag}" });
        }

        var updatedUser = await this.userService.ChangeTag(this.UserId!, tag);

        if (updatedUser is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Tag has changed successfully. New tag {updatedUser.UniqueTag}", null);
            return this.CreatedAtAction(nameof(this.GetUserByTag), new { tag = updatedUser.UniqueTag }, updatedUser);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Cannot change user tag to {tag}", null);
            return this.StatusCode(500, new { Message = $"Failed to change user tag to {tag}" });
        }
    }
}
