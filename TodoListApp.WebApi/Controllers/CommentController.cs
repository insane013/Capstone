using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;
using TodoListApp.Services;
using TodoListApp.WebApi.Helpers.Attributes;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Comments API endpoint.
/// </summary>
[Authorize]
[Route("/Comments")]
public class CommentController : AuthorizedControllerBase
{
    private readonly ICommentDatabaseService commentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="taskService">Database Interaction service instace.</param>
    /// <param name="logger">Logger instance.</param>
    public CommentController(ICommentDatabaseService commentService, ILogger<CommentController> logger)
        : base(logger)
    {
        this.commentService = commentService;
    }

    /// <summary>
    /// Get comments by filter endpoint.
    /// </summary>
    /// <param name="filter">CommentFilter object from query string.</param>
    /// <returns>PaginatedResult with Comment models or error code.</returns>
    [HttpGet]
    [RequireUserId]
    public async Task<IActionResult> GetCommentsAsync([FromQuery] CommentFilter filter)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(filter))
        {
            return this.BadRequest(new { Message = $"Invalid filter format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching Comments with filter:\n{filter}", null);

        var data = await this.commentService.GetAllAsync(this.UserId!, filter);

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
    /// Get comment by its id endpoint.
    /// </summary>
    /// <param name="id">Id of comment.</param>
    /// <returns>Comment model or error code.</returns>
    [HttpGet]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> GetCommentById(long id)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(id))
        {
            return this.BadRequest(new { Message = $"Invalid filter format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting comment with ID = {id}..", null);

        var result = await this.commentService.GetByIdAsync(this.UserId!, id);

        if (result is null)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Requested data with ID = {id} was not found.", null);
            return this.NotFound(new { Message = $"Comment not found." });
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Add new comment endpoint.
    /// </summary>
    /// <param name="model">CommentCreateDto in Json format.</param>
    /// <returns>Added CommentModel or error code.</returns>
    [HttpPost]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Creating new comment:\n{model}", null);

        var entry = await this.commentService.AddAsync(this.UserId!, model);

        if (entry is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Comment created successfully. ID = {entry.Id}", null);
            return this.CreatedAtAction(nameof(this.GetCommentById), new { id = entry.Id }, entry);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, "Failed to add Comment despite valid input.", null);
            return this.StatusCode(500, new { Message = "Failed to add the comment." });
        }
    }

    /// <summary>
    /// Edit comment endpoint.
    /// </summary>
    /// <param name="model">CommentUpdateDto in Json format.</param>
    /// <returns>Updated CommentModel or error code.</returns>
    [HttpPut]
    [RequireUserId]
    [Route("{id:long}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit([FromBody] CommentUpdateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Updating existing comment:\n{model}", null);

        var updatedTodo = await this.commentService.UpdateAsync(this.UserId!, model);

        if (updatedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Comment updated successfully.", null);
            return this.Ok(updatedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Comment with ID = {model.Id} doesn't exist.", null);
            return this.NotFound(new { Message = $"Comment with ID = {model.Id} doesn't exist." });
        }
    }

    /// <summary>
    /// Delete comment endpoint.
    /// </summary>
    /// <param name="model">CommentDeleteDto from query string.</param>
    /// <returns>NoContent if success.</returns>
    [HttpDelete]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteTodoTask(
        [FromQuery] CommentDeleteDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Deleting Comment ID = \n{model.Id}..", null);

        var result = await this.commentService.DeleteAsync(this.UserId!, model);

        if (result)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Comment with ID = {model.Id} successfully deleted.", null);
            return this.NoContent();
        }

        LoggingDelegates.LogWarn(this.Logger, $"Comment with ID = {model.Id} was not found.", null);
        return this.NotFound(new { Message = $"Comment with ID = {model.Id} doesn't exist." });
    }
}
