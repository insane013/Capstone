using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.Helpers;
using TodoListApp.Models;
using TodoListApp.Models.Tag;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.WebApi.Exceptions;
using TodoListApp.Services.WebApi.Mapper;
using static TodoListApp.Models.TodoTask.TodoTaskModel;

namespace TodoListApp.Services.WebApi.TodoList;

/// <summary>
/// TodoList service implementing ITodoListDatabaseService interface.
/// </summary>
public class TodoTaskDatabaseService : SecureDatabaseService, ITodoTaskDatabaseService
{
    private readonly ITagDatabaseService tagService;
    private readonly ITagRepository tagRepository;
    private readonly ILogger<TodoTaskDatabaseService> logger;
    private readonly ITodoTaskRepository todoTaskRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">TodoTask repository instance.</param>
    public TodoTaskDatabaseService(ITagDatabaseService tagService, ITagRepository tagRepository, ILogger<TodoTaskDatabaseService> logger, ITodoTaskRepository todoListRep, ITodoAccessDatabaseService accessService, IMapper mapper)
        : base(accessService)
    {
        this.tagService = tagService;
        this.tagRepository = tagRepository;
        this.logger = logger;
        this.todoTaskRepository = todoListRep;
        this.mapper = mapper;
    }

    public async Task<PaginatedResult<TodoTaskModel>> GetTasksAsync(string userId, TodoTaskFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var now = DateTime.UtcNow;

        var data = await DatabaseExceptionHandler.Execute(
                async () => await this.todoTaskRepository.GetAllAsync());

        if (filter.OnlyAssigned)
        {
            data = data.Where(x => x.AssignedUserId == userId);
        }

        if (filter.TodoListId != 0)
        {
            if (!await this.HasAccessAsync(filter.TodoListId, userId))
            {
                throw new AccessDeniedException($"User {userId} does not have access to TodoList {filter.TodoListId}");
            }

            data = data.Where(x => x.TodoListId == filter.TodoListId);
        }

        if (filter.ShowComplete || filter.ShowOverdue || filter.ShowPending)
        {
            data = data.Where(task =>
                (filter.ShowComplete && task.IsCompleted) ||
                (filter.ShowOverdue && !task.IsCompleted && task.Deadline < now) ||
                (filter.ShowPending && !task.IsCompleted && task.Deadline >= now));
        }

        if (filter.Priorities.Any())
        {
            data = data.Where(x => filter.Priorities.Contains((TaskPriority)x.Priority));
        }

        if (filter.DeadlineBefore != null)
        {
            data = data.Where(x => x.Deadline <= filter.DeadlineBefore);
        }

        if (filter.DeadlineAfter != null)
        {
            data = data.Where(x => x.Deadline >= filter.DeadlineAfter);
        }

        if (!string.IsNullOrEmpty(filter.Tag))
        {
            data = data.Where(x => x.Tags.Any(t => t.Tag == filter.Tag));
        }

        var searchOpt = filter.SearchOptions;

        if (searchOpt is not null)
        {
            if (!string.IsNullOrEmpty(searchOpt.Title))
            {
                data = data.Where(x => x.Title.Contains(searchOpt.Title, StringComparison.CurrentCultureIgnoreCase));
            }

            if (searchOpt.CreatedDate != null)
            {
                var date = searchOpt.CreatedDate.Value;

                var start = date.ToDateTime(TimeOnly.MinValue);
                var end = date.ToDateTime(TimeOnly.MaxValue);

                data = data.Where(t => t.CreatedTime >= start && t.CreatedTime <= end);
            }

            if (searchOpt.Deadline != null)
            {
                var date = searchOpt.Deadline.Value;

                var start = date.ToDateTime(TimeOnly.MinValue);
                var end = date.ToDateTime(TimeOnly.MaxValue);

                data = data.Where(t => t.Deadline >= start && t.Deadline <= end);
            }
        }

        data = filter.SortBy switch
        {
            TodoTaskFilter.SortOption.None => data,
            TodoTaskFilter.SortOption.TitleAsc => data.OrderBy(x => x.Title),
            TodoTaskFilter.SortOption.TitleDesc => data.OrderByDescending(x => x.Title),
            TodoTaskFilter.SortOption.DeadlineAsc => data.OrderBy(x => x.Deadline),
            TodoTaskFilter.SortOption.DeadlineDesc => data.OrderByDescending(x => x.Deadline),
            _ => data
        };

        var result = PaginationHelper.Paginate(
            data,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                var model = this.mapper.Map<TodoTaskModel>(x);
                model.Tags = x.Tags.Select(t => this.mapper.Map<TagModel>(t));
                model.CurrentUserAccessInfo = this.mapper.Map<TodoAccessModel>(x.TodoList.TodoAccesses
                    .FirstOrDefault(t => t.UserId == userId && t.TodoListId == x.TodoListId));

                var list = x.TodoList;

                if (list is not null)
                {
                    model.AvailableTags = list.Tags.Select(t => this.mapper.Map<TagModel>(t));
                }

                return model;
            });

        return result;
    }

    public async Task<TodoTaskModel?> GetOneAsync(string userId, long id)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.GetByIdAsync(id));

        if (entry is null)
        {
            return null;
        }

        if (!await this.HasAccessAsync(entry.TodoListId, userId))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList {entry.TodoListId}");
        }

        var model = this.mapper.Map<TodoTaskModel>(entry);
        model.CurrentUserAccessInfo = this.mapper.Map<TodoAccessModel>(entry.TodoList.TodoAccesses
                    .FirstOrDefault(t => t.UserId == userId && t.TodoListId == entry.TodoListId));

        model.Tags = entry.Tags.Select(t => this.mapper.Map<TagModel>(t));

        var list = entry.TodoList;

        if (list is not null)
        {
            model.AvailableTags = list.Tags.Select(t => this.mapper.Map<TagModel>(t));
        }

        return model;
    }

    public async Task<TodoTaskModel?> AddNewAsync(string userId, TaskCreateDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(model.TodoListId, userId, AccessLevel.Editor))
        {
            throw new AccessDeniedException($"User {userId} does not have access to edit TodoList {model.TodoListId}");
        }

        var tags = new List<TaskTagEntity>();

        foreach (var tagDto in model.Tags)
        {
            var existingTag = await this.tagService.GetAsync(userId, tagDto.TodoListId, tagDto.Tag);
            if (existingTag != null)
            {
                tags.Add(this.mapper.Map<TaskTagEntity>(existingTag));
            }
            else
            {
                var newTag = await this.tagService.AddAsync(userId, tagDto);
                tags.Add(this.mapper.Map<TaskTagEntity>(newTag));
            }
        }

        var entity = this.mapper.Map<TodoTaskEntity>(model);
        entity.Tags.Clear();

        foreach (var t in tags)
        {
            entity.Tags.Add(t);
        }

        entity.AssignedUserId = userId;

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.AddAsync(entity),
            relatedDataNotFoundError: "Related todo list not found.");

        return entry is not null ? this.mapper.Map<TodoTaskModel>(entry) : null;
    }

    public async Task<TodoTaskModel?> UpdateAsync(string userId, TaskUpdateDto model)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (model is null)
        {
            return null;
        }

        _ = await this.CheckAccessDataAsync(model.TodoId, model.TodoListId, userId, AccessLevel.Editor);

        var entity = this.mapper.Map<TodoTaskEntity>(model);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.UpdateAsync(entity),
            relatedDataNotFoundError: "Related todo list not found.");

        return entry is not null ? this.mapper.Map<TodoTaskModel>(entry) : null;
    }

    public async Task<TodoTaskModel?> UpdateTagsAsync(string userId, TaskUpdateTagsDto model)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (model is null)
        {
            return null;
        }

        _ = await this.CheckAccessDataAsync(model.TodoId, model.TodoListId, userId, AccessLevel.Editor);

        var newTags = model.Tags.ToList();

        var text = string.Join("\n", newTags.Select(x => x?.ToString() ?? "null"));
        LoggingDelegates.LogInfo(this.logger, $"Tags:\n{text}", null);

        // because foreach causes warning "Loops Should be simplified using WHERE linq method" I used pragma here
        // tried to do it with Select, but got troubles with parallel accesss to EF DbContext and InvalidOperationException
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var tag in newTags)
        {
            if (await this.tagService.GetAsync(userId, model.TodoListId, tag) is null)
            {
                LoggingDelegates.LogInfo(this.logger, $"Creating tag: {tag}", null);
                _ = await this.tagService.AddAsync(userId, new TagModel { TodoListId = model.TodoListId, Tag = tag });
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

        // beacause of EF tracking. If I create entity using mapper EF provides error "different entities with same id are tracking"
        List<TaskTagEntity> tagsEntity = new List<TaskTagEntity>();

        foreach (var tag in newTags)
        {
            var ent = await this.tagRepository.GetAsync(model.TodoListId, tag);

            if (ent is not null)
            {
                tagsEntity.Add(ent);
            }
        }

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.UpdateTagsAsync(model.TodoId, tagsEntity),
            relatedDataNotFoundError: "Related todo list not found.");

        return entry is not null ? this.mapper.Map<TodoTaskModel>(entry) : null;
    }

    public async Task<TodoTaskModel?> ChangeCompletionState(string userId, TaskCompleteDto model)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(model);

        _ = await this.CheckAccessDataAsync(model.TodoId, model.TodoListId, userId, AccessLevel.AssignedUser);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.UpdateCompletionAsync(model.TodoId, model.Status),
            relatedDataNotFoundError: "Related todo list not found.");

        return entry is not null ? this.mapper.Map<TodoTaskModel>(entry) : null;
    }

    public async Task<TodoTaskModel?> ChangeAssignedUser(string userId, TaskReAssignDto model)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(model);

        _ = await this.CheckAccessDataAsync(model.TodoId, model.TodoListId, userId, AccessLevel.Editor);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.UpdateAssignedUserAsync(model.TodoId, model.OtherUserId),
            relatedDataNotFoundError: "Related todo list not found.");

        return entry is not null ? this.mapper.Map<TodoTaskModel>(entry) : null;
    }

    public async Task<TodoTaskModel?> ChangeTaskPriority(string userId, TaskChangePriorityDto model)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(model);

        _ = await this.CheckAccessDataAsync(model.TodoId, model.TodoListId, userId, AccessLevel.Editor);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.UpdatePriorityAsync(model.TodoId, (int)model.NewPriority),
            relatedDataNotFoundError: "Related todo list not found.");

        return entry is not null ? this.mapper.Map<TodoTaskModel>(entry) : null;
    }

    public async Task<bool> DeleteAsync(string userId, TaskDeleteDto model)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(model);

        _ = await this.CheckAccessDataAsync(model.TodoId, model.TodoListId, userId, AccessLevel.AssignedUser);

        return await DatabaseExceptionHandler.Execute(
            async () => await this.todoTaskRepository.DeleteAsync(model.TodoId));
    }

    private async Task<bool> CheckAccessDataAsync(long taskId, long expectedTodoListId, string userId, AccessLevel accessLevel)
    {
        var entry = await this.todoTaskRepository.GetByIdAsync(taskId);

        if (entry is null || entry.TodoListId != expectedTodoListId)
        {
            throw new AccessDeniedException($"TodoTask #{taskId} doesn't belong to TodoList#{expectedTodoListId}");
        }

        if (accessLevel == AccessLevel.AssignedUser)
        {
            bool isAssigned = entry.AssignedUserId == userId;
            bool hasHigherAccess = await this.HasAccessAsync(expectedTodoListId, userId, AccessLevel.Editor);

            if (!isAssigned && !hasHigherAccess)
            {
                throw new AccessDeniedException($"User {userId} does not have access to TodoList {expectedTodoListId}");
            }
        }

        if (!await this.HasAccessAsync(expectedTodoListId, userId, accessLevel))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList {expectedTodoListId}");
        }

        return true;
    }
}
