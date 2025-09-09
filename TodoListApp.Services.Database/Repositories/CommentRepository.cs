using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for performing CRUD operations on comments related to comments.
/// </summary>
public class CommentRepository : ICommentRepository
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for data access.</param>
    public CommentRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously retrieves all comments related to a specific todotask.
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <returns>A list of comments associated with the specified task.</returns>
    public async Task<IEnumerable<CommentEntity>> GetAllAsync(long taskId)
    {
        return await this.context.Comments
            .Where(x => x.TodoTaskId == taskId)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a comment by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the comment.</param>
    /// <returns>The comment entity if found; otherwise, null.</returns>
    public async Task<CommentEntity?> GetByIdAsync(long id)
    {
        return await this.context.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Asynchronously adds a new comment to the database.
    /// </summary>
    /// <param name="entity">The comment entity to add.</param>
    /// <returns>The added comment entity with generated values.</returns>
    public async Task<CommentEntity?> AddAsync(CommentEntity entity)
    {
        var entry = await this.context.Comments.AddAsync(entity);
        _ = await this.context.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously updates an existing comment in the database.
    /// </summary>
    /// <param name="entity">The comment entity with updated values.</param>
    /// <returns>The updated comment entity if it exists; otherwise, null.</returns>
    public async Task<CommentEntity?> UpdateAsync(CommentEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var entry = await this.context.Comments.FindAsync(entity.Id);
        if (entry == null)
        {
            return null;
        }

        entry.Content = entity.Content;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously deletes a comment from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var entry = await this.context.Comments.FindAsync(id);
        if (entry == null)
        {
            return false;
        }

        _ = this.context.Comments.Remove(entry);
        _ = await this.context.SaveChangesAsync();
        return true;
    }
}
