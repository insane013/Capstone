using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for performing CRUD operations on invites related to todolists.
/// </summary>
public class InviteRepository : IInviteRepository
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="InviteRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for data access.</param>
    public InviteRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously adds a new invite to the database.
    /// </summary>
    /// <param name="entity">The invite entity to add.</param>
    /// <returns>The added invite entity with generated values.</returns>
    public async Task<InviteEntity?> AddAsync(InviteEntity entity)
    {
        var entry = await this.context.Invites.AddAsync(entity);
        _ = await this.context.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously deletes an invite from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the invite to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var entry = await this.context.Invites.FindAsync(id);
        if (entry == null)
        {
            return false;
        }

        _ = this.context.Invites.Remove(entry);
        _ = await this.context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Asynchronously retrieves all invites related to a specific todolist.
    /// </summary>
    /// <param name="todoListId">The ID of the todolist.</param>
    /// <returns>A list of invites associated with the specified todolist.</returns>
    public async Task<IEnumerable<InviteEntity>> GetAllFromListAsync(long todoListId)
    {
        return await this.context.Invites
            .Where(x => x.TodoListId == todoListId)
            .Include(x => x.TodoList)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves all invites sent to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of invites addressed to the specified user.</returns>
    public async Task<IEnumerable<InviteEntity>> GetAllFromUserAsync(string userId)
    {
        return await this.context.Invites
            .Where(x => x.UserId == userId)
            .Include(x => x.TodoList)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves an invite by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the invite.</param>
    /// <returns>The invite entity if found; otherwise, null.</returns>
    public async Task<InviteEntity?> GetByIdAsync(long id)
    {
        return await this.context.Invites
            .Include(x => x.TodoList)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Asynchronously updates an existing invite in the database.
    /// </summary>
    /// <param name="entity">The invite entity with updated values.</param>
    /// <returns>The updated invite entity if it exists; otherwise, null.</returns>
    public async Task<InviteEntity?> UpdateAsync(InviteEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var entry = await this.context.Invites.FindAsync(entity.Id);
        if (entry == null)
        {
            return null;
        }

        entry.Message = entity.Message;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }
}
