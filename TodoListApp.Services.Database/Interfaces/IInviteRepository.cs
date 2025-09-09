using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface with TodoList CRUD operations.
/// </summary>
/// <summary>
/// Interface for managing invites related to todolists.
/// </summary>
public interface IInviteRepository
{
    /// <summary>
    /// Asynchronously retrieves all invites associated with a specific todolist.
    /// </summary>
    /// <param name="todoListId">The ID of the todolist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="InviteEntity"/> objects.</returns>
    Task<IEnumerable<InviteEntity>> GetAllFromListAsync(long todoListId);

    /// <summary>
    /// Asynchronously retrieves all invites sent to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="InviteEntity"/> objects.</returns>
    Task<IEnumerable<InviteEntity>> GetAllFromUserAsync(string userId);

    /// <summary>
    /// Asynchronously retrieves an invite by its ID.
    /// </summary>
    /// <param name="id">The ID of the invite.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="InviteEntity"/>, or null if not found.</returns>
    Task<InviteEntity?> GetByIdAsync(long id);

    /// <summary>
    /// Asynchronously adds a new invite to the database.
    /// </summary>
    /// <param name="entity">The invite entity to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="InviteEntity"/>, or null if the operation failed.</returns>
    Task<InviteEntity?> AddAsync(InviteEntity entity);

    /// <summary>
    /// Asynchronously updates an existing invite in the database.
    /// </summary>
    /// <param name="entity">The invite entity to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="InviteEntity"/>, or null if not found.</returns>
    Task<InviteEntity?> UpdateAsync(InviteEntity entity);

    /// <summary>
    /// Asynchronously deletes an invite by its ID.
    /// </summary>
    /// <param name="id">The ID of the invite to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(long id);
}
