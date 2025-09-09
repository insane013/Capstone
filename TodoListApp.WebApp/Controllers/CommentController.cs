using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;
using TodoListApp.Models.TodoTask;
using TodoListApp.Services.WebApp.Interfaces;
using TodoListApp.WebApp.Helpers;

namespace TodoListApp.WebApp.Controllers;

[Route("Comments/")]
[Authorize]
public class CommentController : BaseController
{
    private readonly ICommentWebApiService commentService;
    private readonly IFilterHandler filterHandler;

    public CommentController(ICommentWebApiService commentService, IFilterHandler filterHandler, ILogger<CommentController> logger)
        : base(logger)
    {
        this.commentService = commentService;
        this.filterHandler = filterHandler;
    }

    [HttpPost]
    [Route("ApplyFilterJson")]
    public IActionResult ApplyFilterJson([FromBody] CommentFilter filter, [FromQuery] Uri? returnUrl, [FromQuery] string filterName = "CommentFilter")
    {
        if (!this.ValidateModel(filter) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "Home", "Home");
        }

        LoggingDelegates.LogInfo(this.Logger, "Applying filter requsted from js..", null);

        return this.filterHandler.Apply(this, filter, returnUrl, filterName);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(CommentCreateDto model)
    {
        if (model is null)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
        }

        LoggingDelegates.LogInfo(this.Logger, $"Creating comment:\n{model}", null);

        var token = this.Request.Cookies["access_token"];

        var created = await this.commentService.AddAsync(model, token!);

        if (created is null)
        {
            this.TempData["ErrorMessage"] = "Something went wrong.";
            return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
        }

        this.TempData["Success"] = $"Comment #{created.Id} successfully created.";

        return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
    }

    [HttpPost]
    [Route("Edit")]
    public async Task<IActionResult> Edit(CommentUpdateDto model)
    {
        if (model is null)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
        }

        this.Logger.LogWarning($"Editing comment: {model}");

        var token = this.Request.Cookies["access_token"];

        var created = await this.commentService.UpdateAsync(model, token!);

        if (created is null)
        {
            this.TempData["ErrorMessage"] = "Something went wrong.";
            return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
        }

        this.TempData["Success"] = $"Comment #{created.Id} successfully updated.";

        return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> Delete(CommentDeleteDto model)
    {
        if (model is null)
        {
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }

        if (!this.ModelState.IsValid)
        {
            return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
        }

        this.Logger.LogWarning($"Deleting comment: {model}");

        var token = this.Request.Cookies["access_token"];

        var result = await this.commentService.DeleteAsync(model, token!);

        if (!result)
        {
            this.TempData["ErrorMessage"] = "Something went wrong.";
            return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
        }

        this.TempData["Success"] = $"Comment #{model.Id} successfully deleted.";

        return this.RedirectToAction("Details", controllerName: "TodoTask", new { id = model.TaskId });
    }
}
