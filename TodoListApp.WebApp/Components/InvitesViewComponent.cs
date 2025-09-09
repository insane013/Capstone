using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models;
using TodoListApp.Models.Invite;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Components;

public class InvitesViewComponent : ViewComponent
{
    private readonly IInviteWebApiService inviteService;
    private readonly ILogger<InvitesViewComponent> logger;

    public InvitesViewComponent(IInviteWebApiService inviteService, ILogger<InvitesViewComponent> logger)
    {
        this.inviteService = inviteService;
        this.logger = logger;
    }

    // todo
    public async Task<IViewComponentResult> InvokeAsync(InviteFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var token = this.Request.Cookies["access_token"];

        this.logger.LogWarning("Invites view component call");

        PaginatedResult<InviteModel> data;

        if (!string.IsNullOrEmpty(filter.ForUser))
        {
            data = await this.inviteService.GetFromUserAsync(filter, token!);
        }
        else
        {
            data = await this.inviteService.GetFromListAsync(filter, token!);
        }

        // todo pag
        return this.View("_AllListInvitesPartial", new InvitesFromTodoWebModel { TodoListId = filter.ForTodoList, Invites = data.Items.ToList() });
    }
}
