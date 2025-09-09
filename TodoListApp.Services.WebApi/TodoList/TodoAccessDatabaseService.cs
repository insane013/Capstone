using AutoMapper;
using TodoListApp.Models.TodoList;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.WebApi.Exceptions;

namespace TodoListApp.Services.WebApi.TodoList;

/// <summary>
/// TodoList service implementing ITodoListDatabaseService interface.
/// </summary>
public class TodoAccessDatabaseService : ITodoAccessDatabaseService
{
    private readonly ITodoAccessRepository todoAccessRepository;
    private readonly ITodoTaskRepository taskRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoAccessDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">Todolist repository instance.</param>
    public TodoAccessDatabaseService(ITodoAccessRepository todoAccessRep, ITodoTaskRepository taskRepository, IMapper mapper)
    {
        this.todoAccessRepository = todoAccessRep;
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public async Task<ICollection<TodoAccessModel>> GetFromTodoListAsync(long todoListId)
    {
        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.todoAccessRepository.GetAllAsync());

        if (data == null)
        {
            return new List<TodoAccessModel>();
        }

        return data.Where(x => x.TodoListId == todoListId)
            .Select(x => this.mapper.Map<TodoAccessModel>(x)).ToList();
    }

    public async Task<ICollection<TodoAccessModel>> GetFromTaskAsync(long taskId)
    {
        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.todoAccessRepository.GetAllAsync());
        var task = await DatabaseExceptionHandler.Execute(
            async () => await this.taskRepository.GetByIdAsync(taskId));

        if (data == null || task == null)
        {
            return new List<TodoAccessModel>();
        }

        return data.Where(x => x.TodoListId == task.TodoListId)
            .Select(x => this.mapper.Map<TodoAccessModel>(x)).ToList();
    }

    public async Task<IEnumerable<TodoAccessModel>> GetFromUserAsync(string userId)
    {
        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.todoAccessRepository.GetAllAsync());

        if (data == null)
        {
            return new List<TodoAccessModel>();
        }

        return data.Where(x => x.UserId == userId)
            .Select(x => this.mapper.Map<TodoAccessModel>(x)).ToList();
    }

    public async Task<TodoAccessModel?> ProvideAccessAsync(TodoAccessModel access)
    {
        ArgumentNullException.ThrowIfNull(access);

        var accessEntity = this.mapper.Map<UserTodoAccess>(access);

        var addedAccess = await DatabaseExceptionHandler.Execute(
            async () => await this.todoAccessRepository.AddAsync(accessEntity),
            relatedDataNotFoundError: "Cannot find related todo list.",
            duplicateError: "User already has access to the todo list");

        if (addedAccess != null)
        {
            return null;
        }

        return this.mapper.Map<TodoAccessModel>(addedAccess);
    }

    public async Task<TodoAccessModel?> ChangeAccessLevelAsync(TodoAccessModel access)
    {
        ArgumentNullException.ThrowIfNull(access);

        var accessEntity = this.mapper.Map<UserTodoAccess>(access);

        var updatedAccess = await DatabaseExceptionHandler.Execute(
            async () => await this.todoAccessRepository.UpdateAsync(accessEntity),
            relatedDataNotFoundError: "Cannot find related todo list.");

        if (updatedAccess != null)
        {
            return null;
        }

        return this.mapper.Map<TodoAccessModel>(updatedAccess);
    }

    public async Task<bool> ForbidAccessAsync(string userId, long todoListId)
    {
        var access = await this.todoAccessRepository.GetAsync(userId, todoListId);

        if (access is null)
        {
            return false;
        }

        _ = await DatabaseExceptionHandler.Execute(
            async () => await this.todoAccessRepository.DeleteAsync(access));

        return true;
    }
}
