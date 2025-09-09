using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for managing todolist entities with CRUD operations.
/// </summary>
public class TodoListRepository : ITodoListRepository
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListRepository"/> class.
    /// </summary>
    /// <param name="context">Database Context.</param>
    public TodoListRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously adds a new todolist to the database.
    /// </summary>
    /// <param name="entity">The todolist entity to add.</param>
    /// <returns>The added todolist entity.</returns>
    public async Task<TodoListEntity?> AddAsync(TodoListEntity entity)
    {
        var entry = await this.context.TodoLists.AddAsync(entity);
        _ = await this.context.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously deletes a todolist by its ID.
    /// </summary>
    /// <param name="id">The ID of the todolist to delete.</param>
    /// <returns>True if deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var entry = await this.context.TodoLists.FindAsync(id);
        if (entry == null)
        {
            return false;
        }

        _ = this.context.TodoLists.Remove(entry);
        _ = await this.context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Asynchronously retrieves all todolist entities with their related data.
    /// </summary>
    /// <returns>A collection of todolist entities with their accesses, tags, and tasks.</returns>
    public async Task<IEnumerable<TodoListEntity>> GetAllAsync()
    {
        return await this.context.TodoLists
            .Include(x => x.TodoAccesses)
            .Include(x => x.Tags)
            .Include(x => x.TodoTasks)
            .ThenInclude(t => t.Tags)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a specific todolist by its ID with related data.
    /// </summary>
    /// <param name="id">The ID of the todolist to retrieve.</param>
    /// <returns>The todolist entity if found; otherwise, null.</returns>
    public async Task<TodoListEntity?> GetByIdAsync(long id)
    {
        return await this.context.TodoLists
            .Include(x => x.TodoAccesses)
            .Include(x => x.Tags)
            .Include(x => x.TodoTasks)
            .ThenInclude(t => t.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Asynchronously updates the title and description of a todolist.
    /// </summary>
    /// <param name="entity">The todolist entity with updated values.</param>
    /// <returns>The updated todolist entity if it exists; otherwise, null.</returns>
    public async Task<TodoListEntity?> UpdateAsync(TodoListEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var entry = await this.context.TodoLists.FindAsync(entity.Id);
        if (entry == null)
        {
            return null;
        }

        entry.Title = entity.Title;
        entry.Description = entity.Description;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously updates the owner of a todolist.
    /// </summary>
    /// <param name="todoListId">The ID of the todolist to update.</param>
    /// <param name="newOwnerId">The ID of the new owner.</param>
    /// <returns>The updated todolist entity if it exists; otherwise, null.</returns>
    public async Task<TodoListEntity?> UpdateOwnerAsync(long todoListId, string newOwnerId)
    {
        var entry = await this.context.TodoLists.FindAsync(todoListId);
        if (entry == null)
        {
            return null;
        }

        entry.OwnerId = newOwnerId;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }
}
