using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApp.Interfaces;
using static TodoListApp.Services.WebApp.Services.UserWebApiService;

namespace TodoListApp.WebApp.Components;

public class UserViewComponent : ViewComponent
{
    private readonly IUserWebApiService userService;
    private readonly ILogger<UserViewComponent> logger;

    public UserViewComponent(IUserWebApiService userService, ILogger<UserViewComponent> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(string userId, UserSearchType type = UserSearchType.Id)
    {
        var token = this.Request.Cookies["access_token"];

        var info = await this.userService.GetUserInfo(userId, token!, type);

        return this.View("_UserInlinePartial", info);
    }
}
