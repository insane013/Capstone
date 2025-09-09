using TodoListApp.Models;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;

namespace TodoListApp.Services;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface ITodoListDatabaseService
{
    /// <summary>
    /// Asyncronously gets list of all todos.
    /// </summary>
    /// <returns>IEnumerable with all todos.</returns>
    public Task<PaginatedResult<TodoListModel>> GetAllAsync(string userId, TodoListFilter filter);

    /// <summary>
    /// Asyncronously gets todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>TodoLsit model or null.</returns>
    public Task<TodoListModel?> GetOneAsync(string userId, long id);

    /// <summary>
    /// Asyncronously creates new todoList.
    /// </summary>
    /// <param name="model">TodoList model.</param>
    /// <returns>Created TodoLsit model or null.</returns>
    public Task<TodoListModel?> AddNewAsync(string userId, TodoListCreateDto model);

    /// <summary>
    /// Asyncronously updates todoList.
    /// </summary>
    /// <param name="model">TodoList model.</param>
    /// <returns>Updated TodoLsit model or null.</returns>
    public Task<TodoListModel?> UpdateAsync(string userId, TodoListUpdateDto model);

    /// <summary>
    /// Asyncronously deletes todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>True if operation successful.</returns>
    public Task<bool> DeleteAsync(string userId, long id);

    public Task<TodoListModel?> ChangeOwnerAsync(string userId, TodoListChangeOwnerDto model);
}
