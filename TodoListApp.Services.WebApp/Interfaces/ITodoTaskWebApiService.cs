using TodoListApp.Models;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;
using TodoListApp.Models.WebApp;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface ITodoTaskWebApiService
{
    /// <summary>
    /// Asyncronously gets list of all todos.
    /// </summary>
    /// <returns>IEnumerable with all todos.</returns>
    public Task<PaginatedResult<TodoTaskModel>> GetAllAsync(TodoTaskFilter filter, string? token);

    /// <summary>
    /// Asyncronously gets todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>TodoLsit model or null.</returns>
    public Task<TodoTaskModel?> GetOneAsync(long id, string? token);

    /// <summary>
    /// Asyncronously creates new todoList.
    /// </summary>
    /// <param name="model">TodoList model.</param>
    /// <returns>Created TodoLsit model or null.</returns>
    public Task<TodoTaskModel?> AddNewAsync(TaskCreateDto model, string? token);

    /// <summary>
    /// Asyncronously updates todoList.
    /// </summary>
    /// <param name="model">TodoList model.</param>
    /// <returns>Updated TodoLsit model or null.</returns>
    public Task<TodoTaskModel?> UpdateAsync(TaskUpdateDto model, string? token);

    /// <summary>
    /// Asyncronously deletes todoList with certain Id.
    /// </summary>
    /// <param name="id">TodoList id.</param>
    /// <returns>True if operation successful.</returns>
    public Task<bool> DeleteAsync(TaskDeleteDto model, string? token);

    /// <summary>
    /// Asyncronously changes task status with certain Id.
    /// </summary>
    /// <param name="id">Task id.</param>
    /// <param name="state">Completion state</param>
    /// <returns>True if operation successful.</returns>
    public Task<bool> ChangeStateAsync(TaskCompleteDto model, string? token);

    public Task<bool> ChangePriorityAsync(TaskChangePriorityDto model, string? token);

    public Task<bool> ChangeAssignedUserAsync(TaskReAssignDto model, string? token);

    public Task<TodoTaskModel?> UpdateTagsAsync(UpdateTagsWebModel model, string? token);
}
