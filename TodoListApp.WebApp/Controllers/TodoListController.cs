using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;
using TodoListApp.WebApp.Helpers;

namespace TodoListApp.WebApp.Controllers;

[Route("TodoLists")]
[Authorize]
public class TodoListController : BaseController
{
    private readonly IFilterHandler filterHandler;
    private readonly ITodoListWebApiService todoListService;

    public TodoListController(IFilterHandler filterHandler, ITodoListWebApiService todoListService, ILogger<TodoListController> logger)
        : base(logger)
    {
        this.filterHandler = filterHandler;
        this.todoListService = todoListService;
    }

    /// <summary>
    /// Apply filter settings obtained from Form.
    /// </summary>
    /// <param name="filter">Filter model.</param>
    /// <param name="returnUrl">Url to redirect after method execution.</param>
    /// <param name="filterName">Filter name.</param>
    /// <returns>Redirect to returnUrl or to default route.</returns>
    [HttpPost]
    [Route("ApplyFilterForm")]
    public IActionResult ApplyFilterForm(TodoListFilter filter, Uri? returnUrl, string filterName = "TodoListFilter")
    {
        if (!this.ValidateModel(filter) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "Home", "Home");
        }

        LoggingDelegates.LogInfo(this.Logger, "Applying filter requsted from form..", null);

        return this.filterHandler.Apply(this, filter, returnUrl, filterName);
    }

    /// <summary>
    /// Apply filter settings obtained as JSON.
    /// </summary>
    /// <param name="filter">Filter model.</param>
    /// <param name="returnUrl">Url to redirect after method execution.</param>
    /// <param name="filterName">Filter name.</param>
    /// <returns>Redirect to returnUrl or to default route.</returns>
    [HttpPost]
    [Route("ApplyFilterJson")]
    public IActionResult ApplyFilterJson([FromBody] TodoListFilter filter, [FromQuery] Uri? returnUrl, [FromQuery] string filterName = "TodoListFilter")
    {
        if (!this.ValidateModel(filter) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "Home", "Home");
        }

        LoggingDelegates.LogInfo(this.Logger, "Applying filter requsted from js..", null);

        return this.filterHandler.Apply(this, filter, returnUrl, filterName);
    }

    /// <summary>
    /// Shows a page with all available to user todoLists.
    /// </summary>
    /// <param name="filter">Todolist Filter.</param>
    /// <returns>Page with todolists.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> AllTodoLists(TodoListFilter filter)
    {
        if (!this.ValidateModel(filter, "Incorrect filter.") || !this.ModelState.IsValid)
        {
            return this.View();
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Fetching todo lists by filter:\n{filter}", null);

                var data = await this.todoListService.GetAllAsync(filter, this.Token);

                LoggingDelegates.LogInfo(this.Logger, $"Todo lists obtained successfully.", null);

                var viewModel = new AllTodoListsWebModel
                {
                    FilterName = "TodoListFilter",
                    Filter = filter,
                    Pagination = new PagingInfo
                    {
                        CurrentPage = data.CurrentPage,
                        PageSize = data.PageSize,
                        TotalPages = data.PageCount,
                    },
                    TodoLists = data.Items,
                };

                return this.View(viewModel);
            },
            this.View());
    }

    /// <summary>
    /// Shows a page with detailed todolist info.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <param name="view">View name.
    /// 'tasks' to show related tasks.
    /// 'invites' to show related invites.</param>
    /// <returns>Page with detailed todolist info.</returns>
    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> Details(long id, string view = "tasks")
    {
        if (!this.ValidateModel(id) || !this.ModelState.IsValid)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Getting details for todo list #{id}", null);

                var data = await this.todoListService.GetOneAsync(id, this.Token);

                this.ViewBag.View = view;

                LoggingDelegates.LogInfo(this.Logger, $"Todolist #{id} info obtained.", null);

                return this.View("TodoListDetails", data);
            },
            this.NotFound(new { Message = "Task not found." }));
    }

    /// <summary>
    /// Geting info to modify todolist.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <param name="returnUrl">Url to return after editing. When null using Referer property value.</param>
    /// <returns>Form to modify todoList.</returns>
    [HttpGet]
    [Route("Edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, Uri? returnUrl = null)
    {
        if (!this.ValidateModel(id) || !this.ModelState.IsValid)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Getting edit info for todo list #{id}", null);

                var data = await this.todoListService.GetOneAsync(id, this.Token);

                var dto = new TodoListUpdateDto
                {
                    TodoListId = data!.TodoListId,
                    Title = data.Title,
                    Description = data.Description,
                };

                this.TempData["ReturnUrl"] = returnUrl ?? new Uri(this.Referer);

                LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);

                return this.View("TodoListEdit", dto);
            },
            this.NotFound(new { Message = "Task not found." }));
    }

    /// <summary>
    /// Post changes in todolist.
    /// </summary>
    /// <param name="model">Updated todolist model.</param>
    /// <param name="returnUrl">Url to return.</param>
    /// <returns>Redirect to return url.</returns>
    [HttpPost]
    [Route("Edit")]
    public async Task<IActionResult> Edit(TodoListUpdateDto model, Uri? returnUrl)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "AllTodoLists", "TodoList");
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Update info for todo list #{model.TodoListId}:\n{model}", null);

                var updated = await this.todoListService.UpdateAsync(model, this.Token);

                this.TempData["Success"] = $"Todolist #{updated?.TodoListId} info successfully updated.";

                LoggingDelegates.LogInfo(this.Logger, $"Todo list #{model.TodoListId} successfully updated.", null);

                return this.RedirectToReturnUrl(returnUrl, "Details", "TodoList", model.TodoListId);
            },
            this.View("TodoListEdit", model));
    }

    /// <summary>
    /// Getting a form to create new todoList.
    /// </summary>
    /// <returns>Form to create new todoList.</returns>
    [HttpGet]
    [Route("Create/")]
    public ViewResult Create()
    {
        LoggingDelegates.LogInfo(this.Logger, $"Creating new todo list...", null);
        return this.View("TodoListCreate", new TodoListCreateDto());
    }

    /// <summary>
    /// Post creating request to api.
    /// </summary>
    /// <param name="model">Todolist model.</param>
    /// <returns>Redirect to todolist details if successed or to list of available todolists.</returns>
    [HttpPost]
    [Route("Create/")]
    public async Task<IActionResult> Create(TodoListCreateDto model)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Creating new todo list:\n{model}", null);

                var created = await this.todoListService.AddNewAsync(model, this.Token);

                this.TempData["Success"] = $"Todolist #{created?.TodoListId} successfully created.";

                LoggingDelegates.LogInfo(this.Logger, $"Todo list #{created?.TodoListId} successfully updated.", null);

                return this.RedirectToAction("Details", new { id = created?.TodoListId });
            },
            this.View("TodoListCreate", model));
    }

    /// <summary>
    /// Get info for todolist requested to delete.
    /// </summary>
    /// <param name="id">Todolist id.</param>
    /// <param name="returnUrl">Url to return after deletion.</param>
    /// <returns>Delete confirmation form.</returns>
    [HttpGet]
    [Route("Delete/{id:long}")]
    public async Task<IActionResult> DeleteGet(long id, Uri? returnUrl = null)
    {
        if (!this.ValidateModel(id) || !this.ModelState.IsValid)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        LoggingDelegates.LogInfo(this.Logger, $"Getting todo list #{id} info for deletion...", null);

        return await this.Execute(
            async () =>
            {
                var data = await this.todoListService.GetOneAsync(id, this.Token);

                LoggingDelegates.LogInfo(this.Logger, $"Data obtained.", null);

                this.TempData["ReturnUrl"] = returnUrl ?? new Uri(this.Referer);

                return this.View("TodoListDelete", data);
            },
            this.NotFound(new { Message = "Task not found." }));
    }

    /// <summary>
    /// Send delete request to API.
    /// </summary>
    /// <param name="id">TodoList Id.</param>
    /// <param name="returnUrl">Url to return after deletion.</param>
    /// <returns>Redirect ot return url.</returns>
    [HttpPost]
    [Route("Delete/{id:long}")]
    public async Task<IActionResult> DeletePost(long id, Uri? returnUrl)
    {
        if (!this.ValidateModel(id) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "AllTodoLists", "TodoList");
        }

        return await this.Execute(
            async () =>
            {
                LoggingDelegates.LogInfo(this.Logger, $"Deleting todo list #{id}.", null);

                _ = await this.todoListService.DeleteAsync(id, this.Token);

                LoggingDelegates.LogInfo(this.Logger, $"Todolist #{id} successfully deleted.", null);

                this.TempData["Success"] = $"Todolist #{id} successfully deleted.";

                return this.RedirectToAction("AllTodoLists");
            },
            this.RedirectToAction("AllTodoLists"));
    }
}
