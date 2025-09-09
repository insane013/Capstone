using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models.User;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Components;

public class ReAssignUserViewComponent : ViewComponent
{
    private readonly IUserWebApiService userService;
    private readonly ILogger<ReAssignUserViewComponent> logger;

    public ReAssignUserViewComponent(IUserWebApiService userService, ILogger<ReAssignUserViewComponent> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(long taskId, long todoListId, string currentUserId)
    {
        var token = this.Request.Cookies["access_token"];

        UserFilter filter = new UserFilter
        {
            PageNumber = 1,
            PageSize = 10,
            TodoListId = todoListId,
        };

        this.logger.LogWarning($"View component call.\n" +
            $"TodoListId: {todoListId}, TaskId: {taskId}\n" +
            $"UserID: {currentUserId}");

        // todo pag
        var users = (await this.userService.GetUsers(filter, token!)).Items.ToList();

        this.logger.LogWarning($"Got {users.Count} users.");

        var model = new ReassignUserWebModel
        {
            TodoId = taskId,
            TodoListId = todoListId,
            CurrentUserId = currentUserId,
        };

        model.SetUsers(users);

        return this.View(model);
    }
}
