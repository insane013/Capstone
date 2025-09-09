using AutoMapper;
using TodoListApp.Models;
using TodoListApp.Models.Tag;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;
using TodoListApp.Models.TodoTask;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.WebApi.Exceptions;
using TodoListApp.Services.WebApi.Mapper;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.Services.WebApi.TodoList;

/// <summary>
/// TodoList service implementing ITodoListDatabaseService interface.
/// </summary>
public class TodoListDatabaseService : SecureDatabaseService, ITodoListDatabaseService
{
    private readonly ITodoListRepository todoListRepository;
    private readonly ITagDatabaseService tagService;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">Todolist repository instance.</param>
    /// <param name="tagService">Tag service instance.</param>
    public TodoListDatabaseService(ITodoListRepository todoListRep, ITodoAccessDatabaseService accessService, ITagDatabaseService tagService, IMapper mapper)
        : base(accessService)
    {
        this.todoListRepository = todoListRep;
        this.tagService = tagService;
        this.mapper = mapper;
    }

    public async Task<PaginatedResult<TodoListModel>> GetAllAsync(string userId, TodoListFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.todoListRepository.GetAllAsync());

        var accessibleLists = new List<TodoListEntity>();

        foreach (var list in data)
        {
            if (await this.HasAccessAsync(list.Id, userId, AccessLevel.Viewer))
            {
                accessibleLists.Add(list);
            }
        }

        var result = PaginationHelper.Paginate(
            accessibleLists,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                var model = this.mapper.Map<TodoListModel>(x);

                model.SetTasks(x.TodoTasks.Select(t =>
                {
                    var task = this.mapper.Map<TodoTaskModel>(t);
                    task.Tags = t.Tags.Select(tag => this.mapper.Map<TagModel>(tag));
                    return task;
                }).ToList());

                model.CurrentUserAccessInfo = this.mapper.Map<TodoAccessModel>(x.TodoAccesses.FirstOrDefault(t => t.UserId == userId && t.TodoListId == x.Id));
                return model;
            });

        return result;
    }

    public async Task<TodoListModel?> GetOneAsync(string userId, long id)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (!await this.HasAccessAsync(id, userId))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList {id}");
        }

        var x = await DatabaseExceptionHandler.Execute(
            async () => await this.todoListRepository.GetByIdAsync(id));

        if (x is null)
        {
            return null;
        }

        var model = this.mapper.Map<TodoListModel>(x);

        model.SetTasks(x.TodoTasks.Select(t =>
        {
            var task = this.mapper.Map<TodoTaskModel>(t);
            task.Tags = t.Tags.Select(tag => this.mapper.Map<TagModel>(tag));
            return task;
        }).ToList());

        model.CurrentUserAccessInfo = this.mapper.Map<TodoAccessModel>(x.TodoAccesses.FirstOrDefault(t => t.UserId == userId && t.TodoListId == x.Id));

        return model;
    }

    public async Task<TodoListModel?> AddNewAsync(string userId, TodoListCreateDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(userId);

        var entity = this.mapper.Map<TodoListEntity>(model);

        entity.OwnerId = userId;

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoListRepository.AddAsync(entity));

        if (entry is not null)
        {
            _ = await this.AccessService.ProvideAccessAsync(
                new TodoAccessModel
                {
                    UserId = entry.OwnerId,
                    TodoId = entry.Id,
                    Role = TodoRole.Owner,
                });

            return this.mapper.Map<TodoListModel>(entity);
        }

        return null;
    }

    public async Task<TodoListModel?> UpdateAsync(string userId, TodoListUpdateDto model)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(model);

        if (!await this.HasAccessAsync(model.TodoListId, userId, AccessLevel.Editor))
        {
            throw new AccessDeniedException($"User {userId} does not have access to edit TodoList {model.TodoListId}");
        }

        var entity = this.mapper.Map<TodoListEntity>(model);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoListRepository.UpdateAsync(entity));

        if (entry is not null)
        {
            var mapped = this.mapper.Map<TodoListModel>(entry);

            mapped.SetTasks(entry.TodoTasks.Select(t =>
            {
                var task = this.mapper.Map<TodoTaskModel>(t);
                task.Tags = t.Tags.Select(tag => this.mapper.Map<TagModel>(tag));
                return task;
            }).ToList());

            return mapped;
        }

        return null;
    }

    public async Task<bool> DeleteAsync(string userId, long id)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (!await this.HasAccessAsync(id, userId, AccessLevel.Owner))
        {
            throw new AccessDeniedException($"User {userId} does not have access to delete TodoList {id}");
        }

        return await DatabaseExceptionHandler.Execute(
            async () =>
            {
                var list = await this.todoListRepository.GetByIdAsync(id);

                if (list is not null && list.Tags.Count > 0)
                {
                    foreach (var tag in list.Tags)
                    {
                        _ = await this.tagService.DeleteAsync(userId, this.mapper.Map<TagModel>(tag));
                    }
                }

                return await this.todoListRepository.DeleteAsync(id);
            });
    }

    public async Task<TodoListModel?> ChangeOwnerAsync(string userId, TodoListChangeOwnerDto model)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(model);

        if (!await this.HasAccessAsync(model.TodoListId, userId, AccessLevel.Owner))
        {
            throw new AccessDeniedException($"User {userId} does not have access to change owner of TodoList {model.TodoListId}");
        }

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoListRepository.UpdateOwnerAsync(model.TodoListId, model.NewOwnerId));

        if (entry is null)
        {
            return null;
        }

        var allAccess = await this.AccessService.GetFromTodoListAsync(model.TodoListId);

        var otherUserCurrentAccess = allAccess.FirstOrDefault(x => x.UserId == model.NewOwnerId);
        var currentUserAccess = allAccess.FirstOrDefault(x => x.UserId == userId);

        if (otherUserCurrentAccess is null)
        {
            throw new AccessDeniedException("User should have access to the list. Invite them before granting Owner rights.");
        }

        otherUserCurrentAccess.Role = TodoRole.Owner;

        if (currentUserAccess is null)
        {
            throw new InvalidOperationException("Current user lost access unexpectedly.");
        }

        currentUserAccess.Role = TodoRole.Editor;

        var updatedModel = this.mapper.Map<TodoListModel>(entry);

        updatedModel.SetTasks(entry.TodoTasks.Select(t =>
        {
            var task = this.mapper.Map<TodoTaskModel>(t);
            task.Tags = t.Tags.Select(tag => this.mapper.Map<TagModel>(tag));
            return task;
        }).ToList());

        return updatedModel;
    }
}
