using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models.User;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Components;

public class AllUsersViewComponent : ViewComponent
{
    private readonly IUserWebApiService userService;
    private readonly ILogger<AllUsersViewComponent> logger;

    public AllUsersViewComponent(IUserWebApiService userService, ILogger<AllUsersViewComponent> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(long todoListId)
    {
        var token = this.Request.Cookies["access_token"];

        var filter = new UserFilter
        {
            TodoListId = todoListId,
        };

        var info = await this.userService.GetUsers(filter, token);

        // todo
        var users = new UsersFromTodoListWebModel(todoListId, info.Items.ToList());

        return this.View(users);
    }
}
