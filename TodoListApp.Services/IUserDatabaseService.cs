using TodoListApp.Models;
using TodoListApp.Models.User;

namespace TodoListApp.Services;

// TODO write proper XML docs

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface IUserDatabaseService
{
    /// <summary>
    /// Asyncronously gets a paginated list of todos.
    /// </summary>
    /// <returns>IEnumerable with all todos.</returns>
    public Task<PaginatedResult<ViewUserInfo>> GetAllAsync(string userId, UserFilter filter);

    /// <summary>
    /// Asyncronously gets todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>TodoLsit model or null.</returns>
    public Task<ViewUserInfo?> GetByIdAsync(string userId);

    public Task<string> GetTagByIdAsync(string userId);

    public Task<string> GetIdByTagAsync(string tag);

    /// <summary>
    /// Asyncronously gets todoList with certain Id.
    /// </summary>
    /// <param name="email">TodoList id.</param>
    /// <returns>TodoLsit model or null.</returns>
    public Task<ViewUserInfo?> GetByEmailAsync(string email);

    public Task<ViewUserInfo?> GetByTagAsync(string tag);

    public Task<ViewUserInfo?> ChangeTag(string userId, string newTag);
}
