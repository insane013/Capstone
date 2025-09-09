using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;
using TodoListApp.WebApp.Helpers;
using static TodoListApp.Models.TodoTask.TodoTaskModel;

namespace TodoListApp.WebApp.Controllers;

[Route("TodoTasks/")]
[Authorize]
public class TodoTaskController : BaseController
{
    private const string FromTagFilterName = "TaggedTaskFilter";

    private readonly IFilterHandler filterHandler;
    private readonly ITodoTaskWebApiService todoTaskService;
    private readonly IFilterStorageService filterStorageService;

    public TodoTaskController(IFilterHandler filterHandler, ITodoTaskWebApiService todoTaskService, IFilterStorageService filterStorageService, ILogger<TodoTaskController> logger)
        : base(logger)
    {
        this.filterHandler = filterHandler;
        this.todoTaskService = todoTaskService;
        this.filterStorageService = filterStorageService;
    }

    [HttpGet]
    [Route("Assigned")]
    public ViewResult AssignedTasks()
    {
        LoggingDelegates.LogInfo(this.Logger, "Assigned tasks requested.", null);
        return this.View();
    }

    [HttpPost]
    [Route("ApplyFilterForm")]
    public IActionResult ApplyFilterForm(TodoTaskFilter filter, Uri? returnUrl, string filterName = "TodoTaskFilter")
    {
        if (!this.ValidateModel(filter) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "Home", "Home");
        }

        LoggingDelegates.LogInfo(this.Logger, "Applying filter requsted from form..", null);

        return this.filterHandler.Apply(this, filter, returnUrl, filterName);
    }

    [HttpPost]
    [Route("ApplyFilterJson")]
    public IActionResult ApplyFilterJson([FromBody] TodoTaskFilter filter, [FromQuery] Uri? returnUrl, [FromQuery] string filterName = "TodoTaskFilter")
    {
        if (!this.ValidateModel(filter) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "Home", "Home");
        }

        LoggingDelegates.LogInfo(this.Logger, "Applying filter requsted from js..", null);

        return this.filterHandler.Apply(this, filter, returnUrl, filterName);
    }

    [HttpPost]
    [Route("ApplySearch")]
    public IActionResult ApplySearch(TaskSearchOptions searchOpt, long? todoListId, Uri? returnUrl, string filterName = "TodoTaskFilter")
    {
        if (!this.ValidateModel(searchOpt) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "AllTodoLists", "TodoList");
        }

        ArgumentNullException.ThrowIfNull(searchOpt);

        LoggingDelegates.LogInfo(this.Logger, $"Applying search:\n{searchOpt}", null);

        var currentFilter = this.filterStorageService.LoadFilter<TodoTaskFilter>(filterName);

        if (currentFilter is null)
        {
            currentFilter = new TodoTaskFilter { TodoListId = todoListId ?? 0 };
        }

        LoggingDelegates.LogInfo(this.Logger, $"Current filter {filterName}:\n{currentFilter}", null);

        currentFilter.SearchOptions = searchOpt;

        this.filterStorageService.SaveFilter(filterName, currentFilter);

        if (returnUrl is null)
        {
            if (currentFilter is not null && currentFilter.TodoListId != 0)
            {
                return this.RedirectToAction("Details", controllerName: "TodoList", new { id = currentFilter.TodoListId });
            }

            return this.RedirectToAction("AssignedTasks", controllerName: "TodoTask");
        }

        return this.Redirect(returnUrl.ToString());
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        if (!this.ValidateModel(id) || !this.ModelState.IsValid)
        {
            return this.Redirect(this.Referer);
        }

        return await this.Execute(
            async () =>
            {
                var data = await this.todoTaskService.GetOneAsync(id, this.Token);

                return this.View("TaskDetails", data);
            },
            this.NotFound());
    }

    [HttpGet]
    [Route("{list:long}/{tag}")]
    public IActionResult TasksFromTag(string tag, long list)
    {
        if (!this.ValidateModel(list) || !this.ModelState.IsValid)
        {
            return this.Redirect(this.Referer);
        }

        var filter = this.filterStorageService.LoadFilter<TodoTaskFilter>(FromTagFilterName);

        if (filter == null)
        {
            filter = new TodoTaskFilter
            {
                Tag = tag,
                TodoListId = list,
            };

            this.filterStorageService.SaveFilter(FromTagFilterName, filter);
        }

        filter.Tag = tag;
        filter.TodoListId = list;

        return this.View("TaggedTasks", filter);
    }

    [HttpGet]
    [Route("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, Uri? returnUrl = null)
    {
        if (!this.ValidateModel(id) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "AssignedTasks", "TodoTask");
        }

        return await this.Execute(
            async () =>
            {
                var data = await this.todoTaskService.GetOneAsync(id, this.Token!);

                var dto = new TaskUpdateDto
                {
                    TodoId = data!.TodoId,
                    TodoListId = data.TodoListId,
                    Title = data.Title,
                    Description = data.Description,
                    Deadline = data.Deadline,
                    Priority = data.Priority,
                };

                this.TempData["ReturnUrl"] = returnUrl ?? new Uri(this.Referer);

                return this.View("TaskEdit", dto);
            },
            this.NotFound());
    }

    [HttpPost]
    [Route("edit/{id:long}")]
    public async Task<IActionResult> Edit(TaskUpdateDto model, Uri? returnUrl)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            return this.View("TaskEdit", model);
        }

        return await this.Execute(
            async () =>
            {
                _ = await this.todoTaskService.UpdateAsync(model, this.Token!);

                this.TempData["Success"] = $"Task #{model.TodoId} info successfully updated.";

                return this.RedirectToReturnUrl(returnUrl, "Details", "TodoTask", model.TodoId);
            },
            this.View("TaskEdit", model));
    }

    [HttpPost]
    [Route("Tags")]
    public async Task<IActionResult> UpdateTags(UpdateTagsWebModel model, Uri returnUrl)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            var tagsErrors = this.ModelState["Tags"]?.Errors;
            if (tagsErrors != null && tagsErrors.Count > 0)
            {
                LoggingDelegates.LogWarn(this.Logger, $"Tag error:\n{tagsErrors[0].ErrorMessage}", null);
            }

            return this.RedirectToReturnUrl(returnUrl, "AssignedTasks", "TodoTask");
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Updating tags:\n{model}", null);

                _ = await this.todoTaskService.UpdateTagsAsync(model, this.Token);

                this.TempData["Success"] = $"Tags in Tsk #{model.TaskId} updated successfully.";
                LoggingDelegates.LogInfo(this.Logger, $"Tags updated successfully.", null);

                return this.RedirectToReturnUrl(returnUrl, "Details", "TodoTask", model.TaskId);
            },
            this.RedirectToReturnUrl(returnUrl, "Details", "TodoTask", model.TaskId));
    }

    [HttpPost]
    [Route("{id:long}/state")]
    public async Task<IActionResult> ChangeCompletionStatus(long todoListId, long todoId, bool state)
    {
        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction("Details", "TodoList", new { id = todoListId });
        }

        var model = new TaskCompleteDto
        {
            TodoListId = todoListId,
            TodoId = todoId,
            Status = state,
        };

        return await this.Execute(
            async () =>
            {
                _ = await this.todoTaskService.ChangeStateAsync(model, this.Token!);

                this.TempData["Success"] = "Task status successfully updated.";

                return this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask");
            },
            this.RedirectToReturnUrl(new Uri(this.Referer), "Details", "TodoTask", todoId));
    }

    [HttpPost]
    [Route("ChangePriority")]
    public async Task<IActionResult> ChangePriority(long todoListId, long todoId, TaskPriority priority)
    {
        LoggingDelegates.LogInfo(this.Logger, $"Change prior:\n ID: {todoId}, List: {todoListId}, Priority: {priority}", null);

        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction("Details", "TodoList", new { id = todoListId });
        }

        var model = new TaskChangePriorityDto
        {
            TodoListId = todoListId,
            TodoId = todoId,
            NewPriority = priority,
        };

        return await this.Execute(
            async () =>
            {
                _ = await this.todoTaskService.ChangePriorityAsync(model, this.Token!);

                this.TempData["Success"] = "Task priority successfully updated.";

                return this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask");
            },
            this.RedirectToReturnUrl(new Uri(this.Referer), "Details", "TodoTask", todoId));
    }

    [HttpGet]
    [Route("create/")]
    public IActionResult Create(long listId, Uri? returnUrl)
    {
        if (!this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask");
        }

        this.TempData["ReturnUrl"] = returnUrl ?? new Uri(this.Referer);
        return this.View("TaskCreate", new TaskCreateDto { TodoListId = listId });
    }

    [HttpPost]
    [Route("create/")]
    public async Task<IActionResult> Create(TaskCreateDto model, Uri? returnUrl)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View("TaskCreate", model);
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogWarn(this.Logger, $"Creating Task:\n{model}", null);

                var created = await this.todoTaskService.AddNewAsync(model, this.Token!);

                this.TempData["Success"] = $"Task #{created?.TodoId} successfully created.";

                return this.RedirectToReturnUrl(returnUrl, "Details", "TodoTask", created?.TodoId);
            },
            this.View("TaskCreate", model));
    }

    [HttpGet]
    [Route("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] Uri? returnUrl)
    {
        if (!this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask");
        }

        return await this.Execute(
            async () =>
            {
                var data = await this.todoTaskService.GetOneAsync(id, this.Token!);

                this.TempData["ReturnUrl"] = returnUrl ?? new Uri(this.Referer);
                return this.View("TaskDelete", data);
            },
            this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask"));
    }

    [HttpPost]
    [Route("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, TaskDeleteDto model, Uri? returnUrl)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            return this.RedirectToAction("Delete", new { id = id, returnUrl = returnUrl ?? new Uri(this.Referer) });
        }

        return await this.Execute(
            async () =>
            {
                _ = await this.todoTaskService.DeleteAsync(model, this.Token!);

                return this.RedirectToReturnUrl(returnUrl, "AssignedTasks", "TodoTask");
            },
            this.RedirectToReturnUrl(returnUrl, "AssignedTasks", "TodoTask"));
    }

    [HttpPost]
    [Route("Reassign")]
    public async Task<IActionResult> ReAssignUser(TaskReAssignDto model)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            if (model is null)
            {
                return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
            }

            return this.RedirectToAction("Details", controllerName: "Task", new { id = model });
        }

        LoggingDelegates.LogWarn(this.Logger, $"Process data:\n{model}", null);

        return await this.Execute(
            async () =>
            {
                _ = await this.todoTaskService.ChangeAssignedUserAsync(model, this.Token!);

                this.TempData["Success"] = $"Task #{model?.TodoId} successfully reassigned.";

                return this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask");
            },
            this.RedirectToReturnUrl(new Uri(this.Referer), "AssignedTasks", "TodoTask"));
    }
}
