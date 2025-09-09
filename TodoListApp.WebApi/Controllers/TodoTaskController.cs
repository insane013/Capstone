using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;
using TodoListApp.Services;
using TodoListApp.WebApi.Helpers.Attributes;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// TodoTask API endpoints.
/// </summary>
[Authorize]
[Route("/TodoTasks")]
public class TodoTaskController : AuthorizedControllerBase
{
    private readonly ITodoTaskDatabaseService todoTaskService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
    /// </summary>
    /// <param name="taskService">Database Interaction service instace.</param>
    /// <param name="logger">Logger instance.</param>
    public TodoTaskController(ITodoTaskDatabaseService taskService, ILogger<TodoTaskController> logger)
        : base(logger)
    {
        this.todoTaskService = taskService;
    }

    /// <summary>
    /// Get all tasks by filter endpoint.
    /// </summary>
    /// <param name="filter">TodoTask filter from query string. Ensure TodoListId or OnlyAssigned property is specified.</param>
    /// <returns>PaginatedResult with todoTasks.</returns>
    [HttpGet]
    [RequireUserId]
    public async Task<IActionResult> GetTasksAsync([FromQuery] TodoTaskFilter filter)
    {
        if (!this.ValidateModel(filter) || !this.ModelState.IsValid)
        {
            return this.BadRequest(new { Message = $"Invalid filter format." });
        }

        if (filter.TodoListId == 0 && !filter.OnlyAssigned)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Invalid Filter format:\n{filter}", null);
            return this.BadRequest(new { Message = $"Please specify either TodoList or OnlyAssigned property in filter." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Fetching Tasks with filter:\n{filter}", null);

        var data = await this.todoTaskService.GetTasksAsync(this.UserId!, filter);

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
    /// Get certain todoTask endpoint.
    /// </summary>
    /// <param name="id">TodoTask ID.</param>
    /// <returns>TodoTask Model or error code.</returns>
    [HttpGet]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> GetTaskById(long id)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(id))
        {
            return this.BadRequest(new { Message = $"Invalid id format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting TodoTask with ID = {id}..", null);

        var result = await this.todoTaskService.GetOneAsync(this.UserId!, id);

        if (result is null)
        {
            LoggingDelegates.LogWarn(this.Logger, $"Requested data with ID = {id} was not found.", null);
            return this.NotFound(new { Message = $"Todo task not found." });
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Add new TodoTask endpoint.
    /// </summary>
    /// <param name="model">TodoTask create Dto in Json format.</param>
    /// <returns>Added TodoTaskModel or error code.</returns>
    [HttpPost]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> AddTodoTask([FromBody] TaskCreateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Creating new Task:\n{model}", null);

        var addedTodo = await this.todoTaskService.AddNewAsync(this.UserId!, model);

        if (addedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"Task created successfully. ID = {addedTodo.TodoId}", null);
            return this.CreatedAtAction(nameof(this.GetTaskById), new { id = addedTodo.TodoId }, addedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, "Failed to add Todo Task despite valid input.", null);
            return this.StatusCode(500, new { Message = "Failed to add the Todo Task." });
        }
    }

    /// <summary>
    /// Edit existing TodoTask endpoint.
    /// </summary>
    /// <param name="model">TaskUpdateDto in Json format.</param>
    /// <returns>Updated TodoTaskModel or error code.</returns>
    [HttpPut]
    [RequireUserId]
    [Route("{id:long}")]
    [Consumes("application/json")]
    public async Task<IActionResult> EditTodoTask([FromBody] TaskUpdateDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Updating existing TodoTask:\n{model}", null);

        var updatedTodo = await this.todoTaskService.UpdateAsync(this.UserId!, model);

        if (updatedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoTask updated successfully.", null);
            return this.Ok(updatedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Todo Task with ID = {model.TodoId} doesn't exist.", null);
            return this.NotFound(new { Message = $"Todo Task with ID = {model.TodoId} doesn't exist." });
        }
    }

    /// <summary>
    /// Update tags in task endpoint.
    /// </summary>
    /// <param name="model">TaskUpdateTagsDto in json format.</param>
    /// <returns>Updated task model or error code.</returns>
    [HttpPut]
    [Route("{id:long}/Tags")]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateTagsOnTask([FromBody] TaskUpdateTagsDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid tags fromat." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Updating tags in existing TodoTask:\n{model}", null);

        var updatedTodo = await this.todoTaskService.UpdateTagsAsync(this.UserId!, model);

        if (updatedTodo is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoTask updated successfully.", null);
            return this.Ok(updatedTodo);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Todo Task with ID = {model.TodoId} doesn't exist.", null);
            return this.NotFound(new { Message = $"Todo Task with ID = {model.TodoId} doesn't exist." });
        }
    }

    /// <summary>
    /// Change completion state of the task enddpoint.
    /// </summary>
    /// <param name="model">TaskCompletedDto in json format.</param>
    /// <returns>TodoTask Model or error code.</returns>
    [HttpPut]
    [RequireUserId]
    [Route("{id:long}/Status")]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangeCompletion([FromBody] TaskCompleteDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, model.ToString(), null);

        var result = await this.todoTaskService.ChangeCompletionState(this.UserId!, model);

        if (result is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoTask Completion state updated successfully.", null);
            return this.Ok(result);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Todo Task with ID = {model.TodoId} doesn't exist.", null);
            return this.NotFound(new { Message = $"Todo Task with ID = {model.TodoId} doesn't exist." });
        }
    }

    /// <summary>
    /// Change task priority level endpoint.
    /// </summary>
    /// <param name="model">TaskChangePriorityDto in json format.</param>
    /// <returns>Updated TaskModel or error code.</returns>
    [HttpPut]
    [RequireUserId]
    [Route("{id:long}/Priority")]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangePriority([FromBody] TaskChangePriorityDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, model.ToString(), null);

        var result = await this.todoTaskService.ChangeTaskPriority(this.UserId!, model);

        if (result is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoTask Priority updated successfully.", null);
            return this.Ok(result);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Todo Task with ID = {model.TodoId} doesn't exist.", null);
            return this.NotFound(new { Message = $"Todo Task with ID = {model.TodoId} doesn't exist." });
        }
    }

    /// <summary>
    /// Change Assigned User endpoint.
    /// </summary>
    /// <param name="model">TaskReAssignDto in json format.</param>
    /// <returns>Updated TaskModel or error code.</returns>
    [HttpPut]
    [Route("{id:long}/Assigned")]
    [RequireUserId]
    [Consumes("application/json")]
    public async Task<IActionResult> ChangeAssignedUser([FromBody] TaskReAssignDto model)
    {
        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data:\n{model}" });
        }

        LoggingDelegates.LogInfo(this.Logger, model.ToString(), null);

        var result = await this.todoTaskService.ChangeAssignedUser(this.UserId!, model);

        if (result is not null)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoTask successfully assigned to user Id = {model.OtherUserId}", null);
            return this.Ok(result);
        }
        else
        {
            LoggingDelegates.LogWarn(this.Logger, $"Todo Task with ID = {model.TodoId} doesn't exist.", null);
            return this.NotFound(new { Message = $"Todo Task with ID = {model.TodoId} doesn't exist." });
        }
    }

    /// <summary>
    /// Delete TodoTask endpoint.
    /// </summary>
    /// <param name="todoListId">Related TodoList Id from query string.</param>
    /// <param name="todoId">Task Id from query string.</param>
    /// <returns>NoContent if success.</returns>
    [HttpDelete]
    [RequireUserId]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteTodoTask(
        [FromQuery] long todoListId,
        [FromQuery] long todoId)
    {
        var model = new TaskDeleteDto
        {
            TodoListId = todoListId,
            TodoId = todoId,
        };

        if (!this.ModelState.IsValid || !this.ValidateModel(model))
        {
            return this.BadRequest(new { Message = $"Invalid data format." });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Deleting TodoTask ID = \n{model.TodoId}..", null);

        var result = await this.todoTaskService.DeleteAsync(this.UserId!, model);

        if (result)
        {
            LoggingDelegates.LogInfo(this.Logger, $"TodoTask with ID = {model.TodoId} successfully deleted.", null);
            return this.NoContent();
        }

        LoggingDelegates.LogWarn(this.Logger, $"TodoTask with ID = {model.TodoId} was not found.", null);
        return this.NotFound(new { Message = $"Todo Task with ID = {model.TodoId} doesn't exist." });
    }
}
