using TodoListApp.Models;
using TodoListApp.Models.User;
using static TodoListApp.Services.WebApp.Services.UserWebApiService;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface IUserWebApiService
{
    public Task<ViewUserInfo?> GetUserInfo(string userId, string? token, UserSearchType type = UserSearchType.Id);

    public Task<PaginatedResult<ViewUserInfo>> GetUsers(UserFilter filter, string? token);
}
