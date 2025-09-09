using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Controllers;

[Route("Users")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserWebApiService userService;

    public UserController(IUserWebApiService userService, ILogger<UserController> logger)
        : base(logger)
    {
        this.userService = userService;
    }

    [HttpGet]
    [Route("{userTag}")]
    public async Task<IActionResult> GetInfo(string userTag)
    {
        return await this.Execute(
            async () =>
            {
                var data = await this.userService.GetUserInfo(userTag, this.Token!);

                return this.View(data);
            },
            this.View(null),
            "Can't get info about user.");
    }
}
