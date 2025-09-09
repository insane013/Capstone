using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Helpers;
using TodoListApp.Services.WebApp.Interfaces;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.WebApp.Components;

/// <summary>
/// Display task list with certain filter.
/// </summary>
public class TasksViewComponent : ViewComponent
{
    private readonly ITodoTaskWebApiService taskService;
    private readonly IFilterStorageService filterService;
    private readonly ILogger<TasksViewComponent> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TasksViewComponent"/> class.
    /// </summary>
    /// <param name="taskService">API task service.</param>
    /// <param name="filterService">filter service.</param>
    /// <param name="logger">logger.</param>
    public TasksViewComponent(ITodoTaskWebApiService taskService, IFilterStorageService filterService, ILogger<TasksViewComponent> logger)
    {
        this.taskService = taskService;
        this.filterService = filterService;
        this.logger = logger;
    }

    /// <summary>
    /// Shows a list with tasks obtained by filter.
    /// </summary>
    /// <param name="jsonFilter">task filter in json format.</param>
    /// <param name="todoListId">todolist Id. overrides todolist Id from filter.</param>
    /// <param name="accessLevel">user's access level. used to hide unavailable controls in view.</param>
    /// <param name="filterName">name of the filter to save.</param>
    /// <param name="view">user or list.</param>
    /// <returns>View with task list.</returns>
    public async Task<IViewComponentResult> InvokeAsync(string jsonFilter, long todoListId, TodoRole accessLevel = TodoRole.Viewer, string filterName = "TodoTaskFilter", string view = "list")
    {
        var filter = JsonSerializer.Deserialize<TodoTaskFilter>(jsonFilter, JsonSerializationHelper.DefaultOptions);

        ArgumentNullException.ThrowIfNull(filter);

        if (view != "user")
        {
            filter.TodoListId = todoListId;
        }

        this.filterService.SaveFilter(filterName, filter);

        var token = this.Request.Cookies["access_token"];

        LoggingDelegates.LogInfo(this.logger, $"Tasks view component call. FilterName = {filterName} Filter:\n{filter}", null);

        PaginatedResult<TodoTaskModel> tasks = new PaginatedResult<TodoTaskModel>();

        try
        {
            tasks = await this.taskService.GetAllAsync(filter, token!);
        }
        catch (ApplicationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
        }

        this.ViewBag.View = view;

        return this.View("_AllTasksPartial", new TaskListWebModel
        {
            Filter = filter,
            FilterName = filterName,
            TodoTasks = tasks.Items,
            Pagination = new PagingInfo
            {
                CurrentPage = tasks.CurrentPage,
                PageSize = tasks.PageSize,
                TotalPages = tasks.PageCount,
            },
            AccessLevel = accessLevel,
        });
    }
}
