using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface with TodoList CRUD operations.
/// </summary>
public interface ITodoTaskRepository
{
    /// <summary>
    /// Asyncroniously gets a list of all todos associated with certain todoList.
    /// </summary>
    /// <param name="todoListId">TodoList id.</param>
    /// <returns>List of all Todos in certain todoList.</returns>
    public Task<IEnumerable<TodoTaskEntity>> GetAllFromListAsync(long todoListId);

    /// <summary>
    /// Asyncroniously gets a list of all todos associated with certain todoList.
    /// </summary>
    /// <returns>List of all Todos in certain todoList.</returns>
    public Task<IEnumerable<TodoTaskEntity>> GetAllAsync();

    /// <summary>
    /// Asyncroniously Gets todoTask by its id.
    /// </summary>
    /// <param name="id">To-do Task Id.</param>
    /// <returns>TodoTask Entity or Null.</returns>
    public Task<TodoTaskEntity?> GetByIdAsync(long id);

    /// <summary>
    /// Asyncroniously Adds new TodoTask to database.
    /// </summary>
    /// <param name="entity">TodoTaskEntity to create.</param>
    /// <returns>Created entity in database or Null.</returns>
    public Task<TodoTaskEntity?> AddAsync(TodoTaskEntity entity);

    /// <summary>
    /// Asyncroniously updates a todoTask in the database.
    /// </summary>
    /// <param name="entity">TodoTaskEntity to update.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskEntity?> UpdateAsync(TodoTaskEntity entity);

    /// <summary>
    /// Asyncroniously updates a tags in task.
    /// </summary>
    /// <param name="taskId">TodoTask Id to update.</param>
    /// <param name="tags">list of new tags.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskEntity?> UpdateTagsAsync(long taskId, IEnumerable<TaskTagEntity> tags);

    /// <summary>
    /// Asyncroniously updates a todoTask completion state.
    /// </summary>
    /// <param name="id">Task to update.</param>
    /// <param name="state">New completion state.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskEntity?> UpdateCompletionAsync(long id, bool state);

    /// <summary>
    /// Asyncroniously changes a user assigned to the task.
    /// </summary>
    /// <param name="id">TodoTask Id to update.</param>
    /// <param name="newUserId">Is of user to assign.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskEntity?> UpdateAssignedUserAsync(long id, string newUserId);

    /// <summary>
    /// Asyncroniously changes task's priority.
    /// </summary>
    /// <param name="id">TodoTask Id to update.</param>
    /// <param name="newPriority">New priority level.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoTaskEntity?> UpdatePriorityAsync(long id, int newPriority);

    /// <summary>
    /// Asyncroniously deletes todoTask by its id.
    /// </summary>
    /// <param name="id">To-do Task Id.</param>
    /// <returns>True if operation was successfull.</returns>
    public Task<bool> DeleteAsync(long id);

    /// <summary>
    /// Asyncroniously gets count of records in DB.
    /// </summary>
    /// <returns>Number of records.</returns>
    public Task<int> CountRecordsAsync();
}
