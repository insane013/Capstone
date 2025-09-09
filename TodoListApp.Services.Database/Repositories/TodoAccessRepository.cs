using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for managing access roles of users to todolist entities.
/// </summary>
public class TodoAccessRepository : ITodoAccessRepository
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoAccessRepository"/> class.
    /// </summary>
    /// <param name="context">Database Context.</param>
    public TodoAccessRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously adds a new user access entry to a todolist.
    /// </summary>
    /// <param name="access">The access entity to add.</param>
    /// <returns>The created access entity.</returns>
    public async Task<UserTodoAccess?> AddAsync(UserTodoAccess access)
    {
        ArgumentNullException.ThrowIfNull(access);

        var entry = (await this.context.AddAsync(access)).Entity;
        _ = await this.context.SaveChangesAsync();

        return entry;
    }

    /// <summary>
    /// Asynchronously deletes a user access entry from a todolist.
    /// </summary>
    /// <param name="access">The access entity to delete.</param>
    /// <returns>True if deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(UserTodoAccess access)
    {
        ArgumentNullException.ThrowIfNull(access);

        var entry = await this.GetAsync(access.UserId, access.TodoListId);
        if (entry == null)
        {
            return false;
        }

        _ = this.context.Accesses.Remove(entry);
        _ = await this.context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Asynchronously retrieves a specific user access entry for a todolist.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="listId">The ID of the todolist.</param>
    /// <returns>The user access entity if found; otherwise, null.</returns>
    public async Task<UserTodoAccess?> GetAsync(string userId, long listId)
    {
        return await this.context.Accesses
            .Include(x => x.TodoList)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TodoListId == listId && x.UserId == userId);
    }

    /// <summary>
    /// Asynchronously retrieves all user access entries for all todolists.
    /// </summary>
    /// <returns>A collection of all user access entities.</returns>
    public async Task<IEnumerable<UserTodoAccess>> GetAllAsync()
    {
        return await this.context.Accesses
            .Include(x => x.TodoList)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously updates the role of a user access entry for a todolist.
    /// </summary>
    /// <param name="access">The access entity with updated role information.</param>
    /// <returns>The updated access entity if it exists; otherwise, null.</returns>
    public async Task<UserTodoAccess?> UpdateAsync(UserTodoAccess access)
    {
        ArgumentNullException.ThrowIfNull(access);

        var entry = await this.GetAsync(access.UserId, access.TodoListId);
        if (entry == null)
        {
            return null;
        }

        entry.Role = access.Role;
        _ = await this.context.SaveChangesAsync();

        return entry;
    }
}
