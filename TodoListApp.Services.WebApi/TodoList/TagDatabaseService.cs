using AutoMapper;
using TodoListApp.Models;
using TodoListApp.Models.Tag;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.WebApi.Exceptions;
using TodoListApp.Services.WebApi.Mapper;

namespace TodoListApp.Services.WebApi.TodoList;

public class TagDatabaseService : SecureDatabaseService, ITagDatabaseService
{
    private readonly ITagRepository tagRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">Todolist repository instance.</param>
    public TagDatabaseService(ITagRepository tagRepository, ITodoAccessDatabaseService accessService, IMapper mapper)
        : base(accessService)
    {
        this.tagRepository = tagRepository;
        this.mapper = mapper;
    }

    public async Task<TagModel?> AddAsync(string userId, TagModel tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(tag.TodoListId, userId, AccessLevel.Editor, CheckingType.FromList))
        {
            throw new AccessDeniedException($"User {userId} does not have access to edit TodoList {tag.TodoListId}");
        }

        var entity = this.mapper.Map<TaskTagEntity>(tag);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.tagRepository.AddAsync(entity),
            relatedDataNotFoundError: "Cannot find related task.",
            duplicateError: "Such tag already exists in this todo list.");

        return entry is not null ? this.mapper.Map<TagModel>(entry) : null;
    }

    public async Task<bool> DeleteAsync(string userId, TagModel tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(tag.TodoListId, userId, AccessLevel.Editor, CheckingType.FromList))
        {
            throw new AccessDeniedException($"User {userId} does not have access to edit TodoList {tag.TodoListId}");
        }

        return await DatabaseExceptionHandler.Execute(
            async () => await this.tagRepository.DeleteAsync(tag.Id));
    }

    public async Task<PaginatedResult<TagModel>> GetAllAsync(string userId, TagFilter filter)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(filter);

        var data = await DatabaseExceptionHandler.Execute(
                async () => await this.tagRepository.GetAllAsync());

        if (filter.FromTodoList != 0)
        {
            data = data.Where(x => x.TodoListId == filter.FromTodoList);
        }

        var accesses = (await this.AccessService.GetFromUserAsync(userId)).Select(x => x.TodoId);

        if (filter.OnlyAvailable)
        {
            data = data.Where(x => accesses.Contains(x.TodoListId));
        }

        var result = PaginationHelper.Paginate(
            data,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                return this.mapper.Map<TagModel>(x);
            });

        return result;
    }

    public async Task<TagModel?> GetAsync(string userId, long todoListId, string tag)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentException.ThrowIfNullOrEmpty(tag);

        if (!await this.HasAccessAsync(todoListId, userId, AccessLevel.Viewer, CheckingType.FromList))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList #{todoListId}");
        }

        var entry = (await this.tagRepository.GetAllAsync()).Where(x => x.TodoListId == todoListId && x.Tag == tag);

        return this.mapper.Map<TagModel>(entry.FirstOrDefault());
    }

    public Task<TagModel?> GetByIdAsync(string userId, long tagId)
    {
        throw new NotImplementedException();
    }

    public Task<TagModel?> UpdateAsync(string userId, TagModel tag)
    {
        throw new NotImplementedException();
    }
}
