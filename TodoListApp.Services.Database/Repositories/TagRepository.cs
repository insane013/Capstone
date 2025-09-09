using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for performing CRUD operations on task tags associated with todolists.
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagRepository"/> class.
    /// </summary>
    /// <param name="context">Database Context.</param>
    public TagRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously retrieves all task tags from the database.
    /// </summary>
    /// <returns>A list of all task tag entities.</returns>
    public async Task<IEnumerable<TaskTagEntity>> GetAllAsync()
    {
        return await this.context.Tags
            .Include(t => t.TodoList)
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a task tag entity by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the task tag.</param>
    /// <returns>The corresponding task tag entity if found; otherwise, null.</returns>
    public async Task<TaskTagEntity?> GetByIdAsync(long id)
    {
        return await this.context.Tags
            .Include(t => t.TodoList)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Asynchronously retrieves a task tag by its todolist ID and tag name.
    /// </summary>
    /// <param name="todoListId">The ID of the todolist.</param>
    /// <param name="tag">The name of the tag.</param>
    /// <returns>The task tag entity if it exists; otherwise, null.</returns>
    public async Task<TaskTagEntity?> GetAsync(long todoListId, string tag)
    {
        return await this.context.Tags
            .Include(t => t.TodoList)
            .FirstOrDefaultAsync(x => x.TodoListId == todoListId && x.Tag == tag);
    }

    /// <summary>
    /// Asynchronously adds a new task tag to the database.
    /// </summary>
    /// <param name="entity">The task tag entity to add.</param>
    /// <returns>The newly added task tag entity.</returns>
    public async Task<TaskTagEntity?> AddAsync(TaskTagEntity entity)
    {
        var entry = await this.context.Tags.AddAsync(entity);
        _ = await this.context.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously updates an existing task tag in the database.
    /// </summary>
    /// <param name="entity">The task tag entity with updated data.</param>
    /// <returns>The updated task tag entity if it exists; otherwise, null.</returns>
    public async Task<TaskTagEntity?> UpdateAsync(TaskTagEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var entry = await this.context.Tags.FindAsync(entity.Id);
        if (entry == null)
        {
            return null;
        }

        entry.Tag = entity.Tag;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously deletes a task tag from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the task tag to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var entry = await this.context.Tags.FindAsync(id);
        if (entry == null)
        {
            return false;
        }

        _ = this.context.Tags.Remove(entry);
        _ = await this.context.SaveChangesAsync();
        return true;
    }
}
