using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for managing TodoTask entities with CRUD operations.
/// </summary>
public class TodoTaskRepository : ITodoTaskRepository
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskRepository"/> class.
    /// </summary>
    /// <param name="context">Database context instance.</param>
    public TodoTaskRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously retrieves all todotasks from a specified todolist.
    /// </summary>
    /// <param name="todoListId">The ID of the todolist to get tasks from.</param>
    /// <returns>A collection of <see cref="TodoTaskEntity"/>.</returns>
    public async Task<IEnumerable<TodoTaskEntity>> GetAllFromListAsync(long todoListId)
    {
        return await this.context.TodoTasks
            .Where(t => t.TodoListId == todoListId)
            .Include(t => t.TodoList)
            .ThenInclude(l => l.Tags)
            .Include(t => t.TodoList)
            .ThenInclude(l => l.TodoAccesses)
            .Include(t => t.Tags)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves all todotasks from a specified todolist.
    /// </summary>
    /// <returns>A collection of <see cref="TodoTaskEntity"/>.</returns>
    public async Task<IEnumerable<TodoTaskEntity>> GetAllAsync()
    {
        return await this.context.TodoTasks
            .Include(t => t.TodoList)
            .ThenInclude(l => l.Tags)
            .Include(t => t.TodoList)
            .ThenInclude(l => l.TodoAccesses)
            .Include(t => t.Tags)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a todotask by its ID.
    /// </summary>
    /// <param name="id">The ID of the todotask.</param>
    /// <returns>The <see cref="TodoTaskEntity"/> if found; otherwise, null.</returns>
    public async Task<TodoTaskEntity?> GetByIdAsync(long id)
    {
        return await this.context.TodoTasks
            .Include(t => t.TodoList)
            .ThenInclude(l => l.Tags)
            .Include(t => t.TodoList)
            .ThenInclude(l => l.TodoAccesses)
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Asynchronously adds a new todotask to the database.
    /// </summary>
    /// <param name="entity">The <see cref="TodoTaskEntity"/> to add.</param>
    /// <returns>The added <see cref="TodoTaskEntity"/> or null if the entity is null.</returns>
    public async Task<TodoTaskEntity?> AddAsync(TodoTaskEntity entity)
    {
        if (entity is null)
        {
            return null;
        }

        foreach (var t in entity.Tags)
        {
            _ = this.context.Attach(t);
        }

        var entry = await this.context.TodoTasks.AddAsync(entity);
        _ = await this.context.SaveChangesAsync();

        return entry.Entity;
    }

    /// <summary>
    /// Asynchronously updates an existing todotask.
    /// </summary>
    /// <param name="entity">The <see cref="TodoTaskEntity"/> with updated values.</param>
    /// <returns>The updated <see cref="TodoTaskEntity"/> or null if not found.</returns>
    public async Task<TodoTaskEntity?> UpdateAsync(TodoTaskEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var entry = await this.context.TodoTasks.FindAsync(entity.Id);

        if (entry == null)
        {
            return null;
        }

        entry.Title = entity.Title;
        entry.Description = entity.Description;
        entry.Deadline = entity.Deadline;
        entry.UpdatedTime = entity.UpdatedTime;
        entry.Priority = entity.Priority;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously updates tags associated with a todotask.
    /// </summary>
    /// <param name="taskId">The ID of the todotask.</param>
    /// <param name="tags">A collection of <see cref="TaskTagEntity"/> to associate with the task.</param>
    /// <returns>The updated <see cref="TodoTaskEntity"/> or null if not found.</returns>
    public async Task<TodoTaskEntity?> UpdateTagsAsync(long taskId, IEnumerable<TaskTagEntity> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var entry = await this.context.TodoTasks
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == taskId);

        if (entry == null)
        {
            return null;
        }

        entry.Tags.Clear();

        foreach (var tag in tags)
        {
            entry.Tags.Add(tag);
        }

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously updates the completion status of a todotask.
    /// </summary>
    /// <param name="id">The ID of the todotask.</param>
    /// <param name="state">The new completion state.</param>
    /// <returns>The updated <see cref="TodoTaskEntity"/> or null if not found.</returns>
    public async Task<TodoTaskEntity?> UpdateCompletionAsync(long id, bool state)
    {
        var entry = await this.context.TodoTasks.FindAsync(id);

        if (entry == null)
        {
            return null;
        }

        entry.IsCompleted = state;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously updates the assigned user of a todotask.
    /// </summary>
    /// <param name="id">The ID of the todotask.</param>
    /// <param name="newUserId">The ID of the new assigned user.</param>
    /// <returns>The updated <see cref="TodoTaskEntity"/> or null if not found.</returns>
    public async Task<TodoTaskEntity?> UpdateAssignedUserAsync(long id, string newUserId)
    {
        var entry = await this.context.TodoTasks.FindAsync(id);

        if (entry == null)
        {
            return null;
        }

        entry.AssignedUserId = newUserId;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously updates the priority of a todotask.
    /// </summary>
    /// <param name="id">The ID of the todotask.</param>
    /// <param name="newPriority">The new priority value.</param>
    /// <returns>The updated <see cref="TodoTaskEntity"/> or null if not found.</returns>
    public async Task<TodoTaskEntity?> UpdatePriorityAsync(long id, int newPriority)
    {
        var entry = await this.context.TodoTasks.FindAsync(id);

        if (entry == null)
        {
            return null;
        }

        entry.Priority = newPriority;

        _ = await this.context.SaveChangesAsync();
        return entry;
    }

    /// <summary>
    /// Asynchronously deletes a todotask by its ID.
    /// </summary>
    /// <param name="id">The ID of the todotask to delete.</param>
    /// <returns>True if deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var entry = await this.context.TodoTasks.FindAsync(id);

        if (entry == null)
        {
            return false;
        }

        _ = this.context.TodoTasks.Remove(entry);
        _ = await this.context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Asynchronously counts the total number of todotasks in the database.
    /// </summary>
    /// <returns>The total count of todotasks.</returns>
    public async Task<int> CountRecordsAsync()
    {
        return await this.context.TodoTasks.CountAsync();
    }
}
