using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;
using TodoListApp.Services;
using TodoListApp.WebApi.Helpers.Attributes;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Invites API endpoints.
/// </summary>
[Authorize]
[Route("/Invites")]
public class InviteController : AuthorizedControllerBase
{
    private readonly IInviteDatabaseService inviteService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InviteController"/> class.
    /// </summary>
    /// <param name="taskService">Database Interaction service instace.</param>
    /// <param name="logger">Logger instance.</param>
    public InviteController(IInviteDatabaseService inviteService, ILogger<InviteController> logger)
        : base(logger)
    {
        this.inviteService = inviteService;
    }

    /// <summary>
    /// Get user pending invites endpoint.
    /// </summary>
    /// <param name="filter">Invite filter from query string.</param>
    /// <returns>PaginatedResult with InviteModels.</returns>
    [HttpGet]
    [RequireUserId]
    [Route("CurrentUser")]
    public async Task<IActionResult> GetUserInvites([FromQuery] InviteFilter filter)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(filter))
        {
            return this.BadRequest(new { Message = "Not valid data passed as filter." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching invites for current user.", null);

        var data = await this.inviteService.GetFromUserAsync(this.UserId!, filter);

        if (data.Items.Any())
        {
            return this.Ok(data);
        }
        else
        {
            return this.NotFound(new { Message = $"No data obtained with this filter." });
        }
    }

    /// <summary>
    /// Get user pending invites endpoint.
    /// </summary>
    /// <param name="filter">Invite filter from query string.</param>
    /// <returns>PaginatedResult with InviteModels.</returns>
    [HttpGet]
    [Route("List")]
    [RequireUserId]
    public async Task<IActionResult> GetTodoListInvites([FromQuery] InviteFilter filter)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(filter))
        {
            return this.BadRequest(new { Message = "Not valid data passed as filter." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching invites for todoList #{filter.ForTodoList}.", null);

        var data = await this.inviteService.GetFromListAsync(this.UserId!, filter);

        if (data.Items.Any())
        {
            return this.Ok(data);
        }
        else
        {
            return this.NotFound(new { Message = $"No data obtained with this filter." });
        }
    }

    /// <summary>
    /// Get Invite by id endpoint.
    /// </summary>
    /// <param name="id">Invite ID.</param>
    /// <returns>InviteModel or error code.</returns>
    [HttpGet]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> GetInviteById(long id)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(id))
        {
            return this.BadRequest(new { Message = "Not valid data passed as an ID." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting invite with ID = {id}..", null);

        var result = await this.inviteService.GetByIdAsync(this.UserId!, id);

        if (result is null)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Requested data with ID = {id} was not found.", null);
            return this.NotFound(new { Message = $"Invite #{id} not found." });
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Create new Invite endpoint.
    /// </summary>
    /// <param name="model">InviteCreateDto in Json format.</param>
    /// <returns>Created InviteModel or error code.</returns>
    [HttpPost]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> AddInvite([FromBody] InviteCreateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = "Not valid data passed." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Creating new Invite:\n{model}", null);

        var entry = await this.inviteService.AddAsync(this.UserId!, model);

        if (entry.Any())
        {
            LoggingDelegates.LogInfo(this.Logger, $"Invites created successfully.", null);
            return this.Created(new Uri("/", UriKind.Relative), entry);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, "Failed to add Invites despite valid input.", null);
            return this.StatusCode(500, new { Message = "Failed to add Invites." });
        }
    }

    /// <summary>
    /// Edit Invite endpoint.
    /// </summary>
    /// <param name="model">InviteUpdateDto in Json format.</param>
    /// <returns>Updated InviteModel or error code.</returns>
    [HttpPut]
    [Route("{id:long}")]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit([FromBody] InviteUpdateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = "Not valid data passed." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Updating existing invite:\n{model}", null);

        var updatedTodo = await this.inviteService.UpdateAsync(this.UserId!, model);

        if (updatedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Invite updated successfully.", null);
            return this.Ok(updatedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Invite with ID = {model.Id} doesn't exist.", null);
            return this.NotFound(new { Message = $"Invite #{model.Id} not found." });
        }
    }

    /// <summary>
    /// Delete invite endpoint.
    /// </summary>
    /// <param name="model">InviteDeleteDto from query string.</param>
    /// <returns>NoContent if success.</returns>
    [HttpDelete]
    [Route("{id:long}")]
    [RequireUserId]
    public async Task<IActionResult> DeleteInvite(
        [FromQuery] InviteDeleteDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = "Not valid data passed." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Deleting Invite ID = \n{model.Id}..", null);

        var result = await this.inviteService.DeleteAsync(this.UserId!, model);

        if (result)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Invite with ID = {model.Id} successfully deleted.", null);
            return this.NoContent();
        }

        LoggingDelegates.LogWarn(this.Logger, $"Invite with ID = {model.Id} was not found.", null);
        return this.NotFound(new { Message = $"Invite #{model.Id} not found." });
    }

    /// <summary>
    /// Respond on invite endpoint.
    /// </summary>
    /// <param name="model">InviteResponseDto in json format.</param>
    /// <returns>NoContent if success or error code.</returns>
    [HttpPost]
    [Route("{id:long}")]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> ResponseInvite([FromBody] InviteResponseDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = "Not valid data passed." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Response to Invite ID = \n{model.Id}..", null);

        var result = await this.inviteService.UserResponseAsync(this.UserId!, model);

        if (result)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Response to Invite #{model.Id} complete.", null);
            return this.NoContent();
        }

        LoggingDelegates.LogWarn(this.Logger, $"Invite with ID = {model.Id} was not found.", null);
        return this.NotFound(new { Message = $"Invite #{model.Id} not found." });
    }
}
