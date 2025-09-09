using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface with TodoList CRUD operations.
/// </summary>
public interface ITodoListRepository
{
    /// <summary>
    /// Asyncroniously gets a list of all todos.
    /// </summary>
    /// <returns>List of all TodoLists.</returns>
    public Task<IEnumerable<TodoListEntity>> GetAllAsync();

    /// <summary>
    /// Asyncroniously Gets todoList by its id.
    /// </summary>
    /// <param name="id">To-do List Id.</param>
    /// <returns>TodoList Entity or Null.</returns>
    public Task<TodoListEntity?> GetByIdAsync(long id);

    /// <summary>
    /// Asyncroniously Adds new TodoList to database.
    /// </summary>
    /// <param name="entity">TodoListEntity to create.</param>
    /// <returns>Created entity in database or Null.</returns>
    public Task<TodoListEntity?> AddAsync(TodoListEntity entity);

    /// <summary>
    /// Asyncroniously updates a todolist in the database.
    /// </summary>
    /// <param name="entity">TodoListEntity to update.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoListEntity?> UpdateAsync(TodoListEntity entity);

    /// <summary>
    /// Asyncroniously deletes todolist by its id.
    /// </summary>
    /// <param name="id">To-do List Id.</param>
    /// <returns>True if operation was successfull.</returns>
    public Task<bool> DeleteAsync(long id);

    /// <summary>
    /// Asyncroniously updates todolist owner in the database.
    /// </summary>
    /// <param name="todoListId">TodoList Id to update.</param>
    /// <param name="newOwnerId">new owner user Id to update.</param>
    /// <returns>Updated entity in database or Null.</returns>
    public Task<TodoListEntity?> UpdateOwnerAsync(long todoListId, string newOwnerId);
}
