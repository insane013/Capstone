using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.Tag;
using TodoListApp.Services;
using TodoListApp.WebApi.Helpers.Attributes;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Tags API endpoints.
/// </summary>
[Route("/Tags")]
[Authorize]
public class TagController : AuthorizedControllerBase
{
    private readonly ITagDatabaseService tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="taskService">Database Interaction service instace.</param>
    /// <param name="logger">Logger instance.</param>
    public TagController(ITagDatabaseService tagService, ILogger<TagController> logger)
        : base(logger)
    {
        this.tagService = tagService;
    }

    /// <summary>
    /// Get list of tags vy filter endpoint.
    /// </summary>
    /// <param name="filter">TagFilter from query string. Ensure FromTodoList or OnlyAvailable property is specified.</param>
    /// <returns>PaginatedResult with TagModel or error code.</returns>
    [HttpGet]
    [RequireUserId]
    public async Task<IActionResult> GetTags([FromQuery] TagFilter filter)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(filter))
        {
            return this.BadRequest(new { Message = $"Incorrect filter format." });
        }

        if (filter.FromTodoList == 0 && !filter.OnlyAvailable)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Invalid Filter format:\n{filter}", null);
            return this.BadRequest(new { Message = $"Please specify TodoList or OnlyAvailable property in filter." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching Tags with filter:\n{filter}", null);

        var data = await this.tagService.GetAllAsync(this.UserId!, filter);

        if (data.Items.Any())
        {
            LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);
            return this.Ok(data);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"No data obtained with this filter.", null);
            return this.NotFound(new { Message = $"No data obtained with this filter." });
        }
    }

    /// <summary>
    /// Get tag by id endpoint.
    /// </summary>
    /// <param name="id">Tag ID.</param>
    /// <returns>Tag Model or error code.</returns>
    [HttpGet]
    [Route("{id:long}")]
    [RequireUserId]
    public async Task<IActionResult> GetTag(long id)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(id))
        {
            return this.BadRequest(new { Message = $"Incorrect id format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting Tag with ID = {id}..", null);

        var result = await this.tagService.GetByIdAsync(this.UserId!, id);

        if (result is null)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Requested data with ID = {id} was not found.", null);
            return this.NotFound(new { Message = $"Tag #{id} not found" });
        }

        return this.Ok(result);
    }
}
