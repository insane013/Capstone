using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface for managing user access permissions to todolists.
/// </summary>
public interface ITodoAccessRepository
{
    /// <summary>
    /// Asynchronously retrieves all user access entries.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The result contains a collection of <see cref="UserTodoAccess"/> objects.</returns>
    Task<IEnumerable<UserTodoAccess>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves a specific user access entry by user ID and todolist ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="listId">The ID of the todolist.</param>
    /// <returns>A task representing the asynchronous operation. The result is the <see cref="UserTodoAccess"/> entry, or null if not found.</returns>
    Task<UserTodoAccess?> GetAsync(string userId, long listId);

    /// <summary>
    /// Asynchronously adds a new user access entry to the database.
    /// </summary>
    /// <param name="access">The user access entry to add.</param>
    /// <returns>A task representing the asynchronous operation. The result is the created <see cref="UserTodoAccess"/> entry, or null if the operation failed.</returns>
    Task<UserTodoAccess?> AddAsync(UserTodoAccess access);

    /// <summary>
    /// Asynchronously updates an existing user access entry in the database.
    /// </summary>
    /// <param name="access">The user access entry to update.</param>
    /// <returns>A task representing the asynchronous operation. The result is the updated <see cref="UserTodoAccess"/> entry, or null if not found.</returns>
    Task<UserTodoAccess?> UpdateAsync(UserTodoAccess access);

    /// <summary>
    /// Asynchronously deletes a user access entry from the database.
    /// </summary>
    /// <param name="access">The user access entry to delete.</param>
    /// <returns>A task representing the asynchronous operation. The result is true if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(UserTodoAccess access);
}
