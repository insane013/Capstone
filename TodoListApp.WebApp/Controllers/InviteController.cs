using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Controllers;

[Route("Invites")]
[Authorize]
public class InviteController : BaseController
{
    private readonly IInviteWebApiService inviteService;

    public InviteController(IInviteWebApiService inviteService, ILogger<InviteController> logger)
        : base(logger)
    {
        this.inviteService = inviteService;
    }

    /// <summary>
    /// Create new Invite controller method.
    /// </summary>
    /// <param name="todoListId">TodoList Id.</param>
    /// <param name="returnUrl">Url to open if something went wrong.</param>
    /// <returns>Create new Invite form.</returns>
    [HttpGet]
    [Route("Create")]
    public IActionResult Create(long todoListId, Uri? returnUrl)
    {
        LoggingDelegates.LogInfo(this.Logger, "Loading create invite form.", null);

        if (!this.ValidateModel(todoListId, "Incorrect todolist Id.") || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "Home", "Home");
        }

        return this.View(new InviteUsersWebModel { TodoListId = todoListId });
    }

    /// <summary>
    /// Post new Invite to API.
    /// </summary>
    /// <param name="model">Invite model obtained from form.</param>
    /// <returns>TodoList details page.</returns>
    [HttpPost]
    [Route("Invite")]
    public async Task<IActionResult> Invite(InviteUsersWebModel model, Uri? returnUrl)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            return this.RedirectToReturnUrl(returnUrl, "AllTodoLists", "TodoList");
        }

        LoggingDelegates.LogInfo(this.Logger, $"Invite users:\n{model}", null);

        return await this.Execute(
            async () =>
            {
                _ = await this.inviteService.InviteAsync(model, this.Token!);

                this.TempData["Success"] = $"Users invited successfully.";

                return this.RedirectToAction("Details", controllerName: "TodoList", new { id = model.TodoListId });
            },
            this.RedirectToReturnUrl(returnUrl, "Details", "TodoList", model.TodoListId));
    }

    /// <summary>
    /// Get invites from current user.
    /// </summary>
    /// <returns>Page with invite list.</returns>
    [HttpGet]
    [Route("User")]
    public async Task<IActionResult> GetFromUser()
    {
        var filter = new InviteFilter();

        LoggingDelegates.LogInfo(this.Logger, "Loading invites from user..", null);

        return await this.Execute(
            async () =>
            {
                var data = await this.inviteService.GetFromUserAsync(filter, this.Token!);

                return this.View("InvitesFromUser", data.Items.ToList()); // todo pag
            },
            this.RedirectToAction("Home", "Home"));
    }

    /// <summary>
    /// Post response to invite.
    /// </summary>
    /// <param name="model">Response model.</param>
    /// <returns>Redirect to user invite list.</returns>
    [HttpPost]
    [Route("Response")]
    public async Task<IActionResult> InviteResponse(InviteResponseDto model)
    {
        if (!this.ValidateModel(model) || !this.ModelState.IsValid)
        {
            return this.RedirectToAction("GetFromUser", controllerName: "Invite");
        }

        LoggingDelegates.LogInfo(this.Logger, "Responding to invite.", null);

        return await this.Execute(
            async () =>
            {
                _ = await this.inviteService.ResponseAsync(model, this.Token!);

                return this.RedirectToAction("GetFromUser", controllerName: "Invite");
            },
            this.RedirectToAction("GetFromUser", controllerName: "Invite"));
    }
}
