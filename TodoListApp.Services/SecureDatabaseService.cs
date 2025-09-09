using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.Services;

public abstract class SecureDatabaseService
{
    protected SecureDatabaseService(ITodoAccessDatabaseService accessService)
    {
        this.AccessService = accessService;
    }

    public enum AccessLevel
    {
        Viewer,
        AssignedUser,
        Editor,
        Owner,
    }

    public enum CheckingType
    {
        FromTask,
        FromList,
    }

    protected ITodoAccessDatabaseService AccessService { get; }

    protected async Task<bool> HasAccessAsync(long id, string userId, AccessLevel level = AccessLevel.Viewer, CheckingType checkingType = CheckingType.FromList)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var accesses = checkingType switch
        {
            CheckingType.FromList => await this.AccessService.GetFromTodoListAsync(id),
            CheckingType.FromTask => await this.AccessService.GetFromTaskAsync(id),
            _ => await this.AccessService.GetFromTodoListAsync(id)
        };

        return level switch
        {
            AccessLevel.Viewer => accesses.Any(x => x.UserId == userId),
            AccessLevel.AssignedUser => accesses.Any(x => x.UserId == userId),
            AccessLevel.Editor => accesses.Any(x => x.UserId == userId &&
                                                    (x.Role == TodoRole.Editor || x.Role == TodoRole.Owner)),
            AccessLevel.Owner => accesses.Any(x => x.UserId == userId && x.Role == TodoRole.Owner),
            _ => false
        };
    }
}
