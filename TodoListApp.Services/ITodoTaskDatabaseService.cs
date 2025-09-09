using TodoListApp.Models;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;

namespace TodoListApp.Services;

/// <summary>
/// Interface with TodoTask service functionality.
/// </summary>
public interface ITodoTaskDatabaseService
{
    /// <summary>
    /// Asyncroniously gets a list of all todos associated with certain todoList.
    /// </summary>
    /// <returns>List of all Todos in certain todoList.</returns>
    public Task<PaginatedResult<TodoTaskModel>> GetTasksAsync(string userId, TodoTaskFilter filter);

    /// <summary>
    /// Asyncroniously Gets todoTask by its id.
    /// </summary>
    /// <param name="id">To-do Task Id.</param>
    /// <returns>TodoTask Entity or Null.</returns>
    public Task<TodoTaskModel?> GetOneAsync(string userId, long id);

    /// <summary>
    /// Asyncroniously Adds new TodoTask to database.
    /// </summary>
    /// <param name="model">TaskCreateDto.</param>
    /// <returns>Created entity in database or Null.</returns>
    public Task<TodoTaskModel?> AddNewAsync(string userId, TaskCreateDto model);

    /// <summary>
    /// Asyncroniously updates a todoTask in the database.
    /// </summary>
    /// <param name="model">TaskUpdateDto to update.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskModel?> UpdateAsync(string userId, TaskUpdateDto model);

    public Task<TodoTaskModel?> UpdateTagsAsync(string userId, TaskUpdateTagsDto model);

    /// <summary>
    /// Asyncroniously updates a todoTask's completion state.
    /// </summary>
    /// <param name="id">Task Id to update.</param>
    /// <param name="state">New state. True to complete.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskModel?> ChangeCompletionState(string userId, TaskCompleteDto model);

    public Task<TodoTaskModel?> ChangeAssignedUser(string userId, TaskReAssignDto model);

    public Task<TodoTaskModel?> ChangeTaskPriority(string userId, TaskChangePriorityDto model);

    /// <summary>
    /// Asyncroniously deletes todoTask by its id.
    /// </summary>
    /// <param name="id">To-do Task Id.</param>
    /// <returns>True if operation was successfull.</returns>
    public Task<bool> DeleteAsync(string userId, TaskDeleteDto model);
}
