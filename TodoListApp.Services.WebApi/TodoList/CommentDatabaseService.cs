using AutoMapper;
using TodoListApp.Models;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.WebApi.Exceptions;
using TodoListApp.Services.WebApi.Mapper;

namespace TodoListApp.Services.WebApi.TodoList;

public class CommentDatabaseService : SecureDatabaseService, ICommentDatabaseService
{
    private readonly ICommentRepository commentRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">Todolist repository instance.</param>
    public CommentDatabaseService(ICommentRepository commentRepository, ITodoAccessDatabaseService accessService, IMapper mapper)
        : base(accessService)
    {
        this.commentRepository = commentRepository;
        this.mapper = mapper;
    }

    public async Task<CommentModel?> AddAsync(string userId, CommentCreateDto comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(comment.TaskId, userId, AccessLevel.Editor, CheckingType.FromTask))
        {
            throw new AccessDeniedException($"User {userId} does not have access to edit Task {comment.TaskId}");
        }

        var entity = this.mapper.Map<CommentEntity>(comment);

        entity.CreatedUserId = userId;
        entity.CreatedTime = DateTime.UtcNow;

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.commentRepository.AddAsync(entity),
            relatedDataNotFoundError: "Cannot find related task.");

        return entry is not null ? this.mapper.Map<CommentModel>(entry) : null;
    }

    public async Task<bool> DeleteAsync(string userId, CommentDeleteDto comment)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(comment);

        _ = await this.CheckAccessDataAsync(comment.Id, comment.TaskId, userId, AccessLevel.Owner);

        return await DatabaseExceptionHandler.Execute(
            async () => await this.commentRepository.DeleteAsync(comment.Id));
    }

    public async Task<PaginatedResult<CommentModel>> GetAllAsync(string userId, CommentFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(filter.TaskId, userId, AccessLevel.Viewer, CheckingType.FromTask))
        {
            throw new AccessDeniedException($"User {userId} does not have access to Task {filter.TaskId}");
        }

        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.commentRepository.GetAllAsync(filter.TaskId));

        var result = PaginationHelper.Paginate(
            data,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                return this.mapper.Map<CommentModel>(x);
            });

        return result;
    }

    public async Task<CommentModel?> GetByIdAsync(string userId, long commentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.commentRepository.GetByIdAsync(commentId));

        if (entry is null)
        {
            return null;
        }

        if (!await this.HasAccessAsync(entry.TodoTaskId, userId, AccessLevel.Viewer, CheckingType.FromTask))
        {
            throw new AccessDeniedException($"User {userId} does not have access to Task {entry.TodoTaskId}");
        }

        return this.mapper.Map<CommentModel>(entry);
    }

    public async Task<CommentModel?> UpdateAsync(string userId, CommentUpdateDto comment)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (comment is null)
        {
            return null;
        }

        _ = await this.CheckAccessDataAsync(comment.Id, comment.TaskId, userId, AccessLevel.Owner);

        var entity = this.mapper.Map<CommentEntity>(comment);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.commentRepository.UpdateAsync(entity),
            relatedDataNotFoundError: "Cannot find related task.");

        return entry is not null ? this.mapper.Map<CommentModel>(entry) : null;
    }

    private async Task<bool> CheckAccessDataAsync(long commentId, long expectedTaskId, string userId, AccessLevel accessLevel)
    {
        var entry = await this.commentRepository.GetByIdAsync(commentId);

        if (entry is null || entry.TodoTaskId != expectedTaskId)
        {
            throw new AccessDeniedException($"Comment #{commentId} doesn't belong to Task #{expectedTaskId}");
        }

        if (accessLevel == AccessLevel.AssignedUser)
        {
            bool isAssigned = entry.CreatedUserId == userId;
            bool hasHigherAccess = await this.HasAccessAsync(expectedTaskId, userId, AccessLevel.Editor, CheckingType.FromTask);

            if (!isAssigned && !hasHigherAccess)
            {
                throw new AccessDeniedException($"User {userId} does not have access to Task #{expectedTaskId}");
            }
        }

        if (!await this.HasAccessAsync(expectedTaskId, userId, accessLevel, CheckingType.FromTask))
        {
            throw new AccessDeniedException($"User {userId} does not have access to Task #{expectedTaskId}");
        }

        return true;
    }
}
