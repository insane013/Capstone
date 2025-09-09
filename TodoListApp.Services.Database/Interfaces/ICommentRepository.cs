using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface for accessing and manipulating comment data in the data store.
/// </summary>
public interface ICommentRepository
{
    /// <summary>
    /// Retrieves all comments associated with a specific task.
    /// </summary>
    /// <param name="taskId">The ID of the task to retrieve comments for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="CommentEntity"/> objects.</returns>
    Task<IEnumerable<CommentEntity>> GetAllAsync(long taskId);

    /// <summary>
    /// Retrieves a comment by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the comment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="CommentEntity"/>, or null if not found.</returns>
    Task<CommentEntity?> GetByIdAsync(long id);

    /// <summary>
    /// Adds a new comment to the data store.
    /// </summary>
    /// <param name="entity">The comment entity to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added <see cref="CommentEntity"/>.</returns>
    Task<CommentEntity?> AddAsync(CommentEntity entity);

    /// <summary>
    /// Updates an existing comment in the data store.
    /// </summary>
    /// <param name="entity">The comment entity to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="CommentEntity"/> or null if not found.</returns>
    Task<CommentEntity?> UpdateAsync(CommentEntity entity);

    /// <summary>
    /// Deletes a comment by its ID.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(long id);
}
