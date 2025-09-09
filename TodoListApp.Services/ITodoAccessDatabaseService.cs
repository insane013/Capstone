using TodoListApp.Models.TodoList;

namespace TodoListApp.Services;

// TODO write proper XML docs

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface ITodoAccessDatabaseService
{
    public Task<ICollection<TodoAccessModel>> GetFromTodoListAsync(long todoListId);

    public Task<ICollection<TodoAccessModel>> GetFromTaskAsync(long taskId);

    public Task<IEnumerable<TodoAccessModel>> GetFromUserAsync(string userId);

    public Task<TodoAccessModel?> ProvideAccessAsync(TodoAccessModel access);

    public Task<TodoAccessModel?> ChangeAccessLevelAsync(TodoAccessModel access);

    public Task<bool> ForbidAccessAsync(string userId, long todoListId);
}
