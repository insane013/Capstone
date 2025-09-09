using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;
using TodoListApp.Services;
using TodoListApp.WebApi.Helpers.Attributes;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// TodoList API endpoints.
/// </summary>
[Authorize]
[Route("/TodoLists")]
public class TodoListController : AuthorizedControllerBase
{
    private readonly ITodoListDatabaseService todoListService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">Todolist Database Interaction service instace.</param>
    /// <param name="logger">Logger instance.</param>
    public TodoListController(ITodoListDatabaseService todoListService, ILogger<TodoListController> logger)
        : base(logger)
    {
        this.todoListService = todoListService;
    }

    /// <summary>
    /// Get all lists endpoint.
    /// </summary>
    /// <param name="filter">TodoList filter from query string.</param>
    /// <returns>PaginatedResult with TodoListModel or error code.</returns>
    [HttpGet]
    [RequireUserId]
    public async Task<IActionResult> GetTodoListsAsync([FromQuery] TodoListFilter filter)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(filter))
        {
            return this.BadRequest(new { Message = $"Invalid filter format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching TodoLists with filter:\n{filter}", null);

        var data = await this.todoListService.GetAllAsync(this.UserId!, filter);

        if (data.Items.Any())
        {
            LoggingDelegates.LogInfo(this.Logger, $"Todo list fetching success.", null);
            return this.Ok(data);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"No data obtained with this filter:\n{filter}", null);
            return this.NotFound(new { Message = $"No data obtained with this filter." });
        }
    }

    /// <summary>
    /// Get certain todoList by id endpoint.
    /// </summary>
    /// <param name="id">TodoList ID.</param>
    /// <returns>TodoList Model or error code.</returns>
    [HttpGet]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> GetTodoListById(long id)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(id))
        {
            return this.BadRequest(new { Message = $"Invalid ID: {id}" });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting TodoList with ID = {id}..", null);

        var result = await this.todoListService.GetOneAsync(this.UserId!, id);

        if (result is null)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Requested data with ID = {id} was not found.", null);
            return this.NotFound(new { Message = $"Todo list #{id} not found." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Todo list #{id} info obtained.", null);
        return this.Ok(result);
    }

    /// <summary>
    /// Add new TodoList endpoint.
    /// </summary>
    /// <param name="model">TodoList creating model in Json format.</param>
    /// <returns>Added todoList Model or error code.</returns>
    [HttpPost]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> AddTodoList([FromBody] TodoListCreateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Creating a new TodoList:\n{model}", null);

        var addedTodo = await this.todoListService.AddNewAsync(this.UserId!, model);

        if (addedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoList created successfully. ID = {addedTodo.TodoListId}", null);
            return this.CreatedAtAction(nameof(this.GetTodoListById), new { id = addedTodo.TodoListId }, addedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, "Failed to add Todo list despite valid input.", null);
            return this.StatusCode(500, new { Message = "Failed to add the Todo list." });
        }
    }

    /// <summary>
    /// Edit existing TodoList endpoint.
    /// </summary>
    /// <param name="model">Updated TodoList model in Json format.</param>
    /// <returns>Updated todoList Model or error code.</returns>
    [HttpPut]
    [RequireUserId]
    [Route("{id:long}")]
    [Consumes("application/json")]
    public async Task<IActionResult> EditTodoList([FromBody] TodoListUpdateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Updating existing TodoList:\n{model}", null);

        var updatedTodo = await this.todoListService.UpdateAsync(this.UserId!, model);

        if (updatedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoList updated successfully.", null);
            return this.Ok(updatedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, "Failed to update Todo list despite valid input.", null);
            return this.StatusCode(500, new { Message = "Failed to update the Todo list." });
        }
    }

    /// <summary>
    /// Change toddoList Owner endpoint.
    /// </summary>
    /// <param name="model">TodoListChangeOwnerDto in json format.</param>
    /// <returns>Updated todolist model or errror code.</returns>
    [HttpPut]
    [Route("{id:long}/Owner")]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangeOwner([FromBody] TodoListChangeOwnerDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Changing owner:\n{model}", null);

        var updatedTodo = await this.todoListService.ChangeOwnerAsync(this.UserId!, model);

        if (updatedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Owner changed successfully.", null);
            return this.Ok(updatedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, "Failed to change owner despite valid input.", null);
            return this.StatusCode(500, new { Message = "Failed to change owner." });
        }
    }

    /// <summary>
    /// Deleting existing TodoList endpoint.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>NoContent if success.</returns>
    [HttpDelete]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteTodoList(long id)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(id))
        {
            return this.BadRequest(new { Message = $"Invalid Id format: {id}" });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Deleting TodoList ID = {id}..", null);

        var result = await this.todoListService.DeleteAsync(this.UserId!, id);

        if (result)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoList with ID = {id} successfully deleted.", null);
            return this.NoContent();
        }

        LoggingDelegates.LogWarn(this.Logger, $"TodoList with ID = {id} was not found.", null);
        return this.NotFound(new { Message = $"TodoList #{id} doesn't exist." });
    }
}
