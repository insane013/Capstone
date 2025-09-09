using TodoListApp.Models;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface ITodoListWebApiService
{
    /// <summary>
    /// Asyncronously gets a paginated list of todos.
    /// </summary>
    /// <returns>IEnumerable with all todos.</returns>
    public Task<PaginatedResult<TodoListModel>> GetAllAsync(TodoListFilter filter, string? token);

    /// <summary>
    /// Asyncronously gets todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>TodoLsit model or null.</returns>
    public Task<TodoListModel?> GetOneAsync(long id, string? token);

    /// <summary>
    /// Asyncronously creates new todoList.
    /// </summary>
    /// <param name="model">TodoList model.</param>
    /// <returns>Created TodoLsit model or null.</returns>
    public Task<TodoListModel?> AddNewAsync(TodoListCreateDto model, string? token);

    /// <summary>
    /// Asyncronously updates todoList.
    /// </summary>
    /// <param name="model">TodoList model.</param>
    /// <returns>Updated TodoLsit model or null.</returns>
    public Task<TodoListModel?> UpdateAsync(TodoListUpdateDto model, string? token);

    /// <summary>
    /// Asyncronously deletes todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>True if operation successful.</returns>
    public Task<bool> DeleteAsync(long id, string? token);
}
