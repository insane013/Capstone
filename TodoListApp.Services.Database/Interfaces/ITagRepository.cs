using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface for managing task tags in the database.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Asynchronously retrieves all task tags.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The result contains a collection of <see cref="TaskTagEntity"/> objects.</returns>
    Task<IEnumerable<TaskTagEntity>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves a tag by its ID.
    /// </summary>
    /// <param name="id">The ID of the tag.</param>
    /// <returns>A task representing the asynchronous operation. The result is the <see cref="TaskTagEntity"/>, or null if not found.</returns>
    Task<TaskTagEntity?> GetByIdAsync(long id);

    /// <summary>
    /// Asynchronously retrieves a tag by its todolist ID and tag value.
    /// </summary>
    /// <param name="todoListId">The ID of the todolist.</param>
    /// <param name="tag">The tag value.</param>
    /// <returns>A task representing the asynchronous operation. The result is the <see cref="TaskTagEntity"/>, or null if not found.</returns>
    Task<TaskTagEntity?> GetAsync(long todoListId, string tag);

    /// <summary>
    /// Asynchronously adds a new task tag to the database.
    /// </summary>
    /// <param name="entity">The tag entity to add.</param>
    /// <returns>A task representing the asynchronous operation. The result is the created <see cref="TaskTagEntity"/>, or null if the operation failed.</returns>
    Task<TaskTagEntity?> AddAsync(TaskTagEntity entity);

    /// <summary>
    /// Asynchronously updates an existing task tag in the database.
    /// </summary>
    /// <param name="entity">The tag entity to update.</param>
    /// <returns>A task representing the asynchronous operation. The result is the updated <see cref="TaskTagEntity"/>, or null if not found.</returns>
    Task<TaskTagEntity?> UpdateAsync(TaskTagEntity entity);

    /// <summary>
    /// Asynchronously deletes a task tag by its ID.
    /// </summary>
    /// <param name="id">The ID of the tag to delete.</param>
    /// <returns>A task representing the asynchronous operation. The result is true if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(long id);
}
