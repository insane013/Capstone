using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
[Route("ComponentLoader")]
public class ComponentLoaderController : Controller
{
    public ComponentLoaderController(ILogger<ComponentLoaderController> logger)
    {
        this.Logger = logger;
    }

    public ILogger<ComponentLoaderController> Logger { get; }

    [HttpGet]
    [Route("ReAssignUser")]
    public IActionResult ReAssignUser(long taskId, long todoListId, string currentUserId)
    {
        this.Logger.LogWarning($"Reassign User Controller method.\nTask: {taskId}, List: {todoListId}\nCurrent Assigned: {currentUserId}");
        return this.ViewComponent("ReAssignUser", new { taskId, todoListId, currentUserId });
    }
}
