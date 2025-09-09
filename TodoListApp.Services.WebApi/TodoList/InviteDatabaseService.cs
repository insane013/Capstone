using AutoMapper;
using Microsoft.Extensions.Logging;
using TodoListApp.Helpers;
using TodoListApp.Models;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;
using TodoListApp.Models.TodoList;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.WebApi.Exceptions;
using TodoListApp.Services.WebApi.Mapper;

namespace TodoListApp.Services.WebApi.TodoList;

public class InviteDatabaseService : SecureDatabaseService, IInviteDatabaseService
{
    private readonly ILogger<InviteDatabaseService> logger;
    private readonly IInviteRepository inviteRepository;
    private readonly IUserDatabaseService userService;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="InviteDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">Todolist repository instance.</param>
    public InviteDatabaseService(ILogger<InviteDatabaseService> logger, IInviteRepository inviteRepository, IUserDatabaseService userService, ITodoAccessDatabaseService accessService, IMapper mapper)
        : base(accessService)
    {
        this.logger = logger;
        this.inviteRepository = inviteRepository;
        this.userService = userService;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<InviteModel>> AddAsync(string userId, InviteCreateDto invite)
    {
        ArgumentNullException.ThrowIfNull(invite);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(invite.TodoListId, userId, AccessLevel.Owner))
        {
            throw new AccessDeniedException($"User {userId} does not have access to invite users to TodoList {invite.TodoListId}");
        }

        var entities = this.mapper.Map<List<InviteEntity>>(invite);

        List<InviteEntity?> entries = new List<InviteEntity?>();

        entries = await DatabaseExceptionHandler.Execute(
            async () =>
            {
                foreach (var entity in entities)
                {
                    var userInfo = await this.userService.GetByEmailAsync(entity.UserId);
                    LoggingDelegates.LogWarn(this.logger, $"Email: {userInfo?.Email}\nUserId: {userInfo?.UserId}", null);
                    entity.UserId = userInfo is not null ? userInfo.UserId : throw new InvalidOperationException("User with this email doesn't exist.");
                    entries.Add(await this.inviteRepository.AddAsync(entity));
                }

                return entries;
            },
            duplicateError: "Such invite already exist. Delete it or wait for response from user.",
            relatedDataNotFoundError: "Cannot find related todo list.");

        return entries.Count > 0 ? entries.Select(x => this.mapper.Map<InviteModel>(x)) : Enumerable.Empty<InviteModel>();
    }

    public async Task<bool> DeleteAsync(string userId, InviteDeleteDto invite)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(invite);

        _ = await this.CheckAccessDataAsync(invite.Id, invite.TodoListId, userId, AccessLevel.Owner);

        return await DatabaseExceptionHandler.Execute(
            async () => await this.inviteRepository.DeleteAsync(invite.Id));
    }

    public async Task<PaginatedResult<InviteModel>> GetFromListAsync(string userId, InviteFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (!await this.HasAccessAsync(filter.ForTodoList, userId, AccessLevel.Viewer))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList {filter.ForTodoList}");
        }

        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.inviteRepository.GetAllFromListAsync(filter.ForTodoList));

        if (!string.IsNullOrEmpty(filter.ForUser))
        {
            data = data.Where(x => x.UserId == filter.ForUser);
        }

        var result = PaginationHelper.Paginate(
            data,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                return this.mapper.Map<InviteModel>(x);
            });

        return result;
    }

    public async Task<PaginatedResult<InviteModel>> GetFromUserAsync(string userId, InviteFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var data = await DatabaseExceptionHandler.Execute(
            async () => await this.inviteRepository.GetAllFromUserAsync(userId));

        var result = PaginationHelper.Paginate(
            data,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                return this.mapper.Map<InviteModel>(x);
            });

        return result;
    }

    public async Task<InviteModel?> GetByIdAsync(string userId, long inviteId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.inviteRepository.GetByIdAsync(inviteId));

        if (entry is null)
        {
            return null;
        }

        if (!await this.HasAccessAsync(entry.TodoListId, userId, AccessLevel.Viewer))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList {entry.TodoListId}");
        }

        return this.mapper.Map<InviteModel>(entry);
    }

    public async Task<InviteModel?> UpdateAsync(string userId, InviteUpdateDto invite)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (invite is null)
        {
            return null;
        }

        _ = await this.CheckAccessDataAsync(invite.Id, invite.TodoListId, userId, AccessLevel.Owner);

        var entity = this.mapper.Map<InviteEntity>(invite);

        var entry = await DatabaseExceptionHandler.Execute(
            async () => await this.inviteRepository.UpdateAsync(entity),
            relatedDataNotFoundError: "Cannot find related todo list.");

        return entry is not null ? this.mapper.Map<InviteModel>(entry) : null;
    }

    public async Task<bool> UserResponseAsync(string userId, InviteResponseDto invite)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(invite);

        _ = await this.CheckAccessDataAsync(invite.Id, invite.TodoListId, userId, AccessLevel.AssignedUser);

        if (invite.Accepted)
        {
            _ = await this.AccessService.ProvideAccessAsync(new TodoAccessModel()
            {
                UserId = userId,
                TodoId = invite.TodoListId,
                Role = TodoAccessModel.TodoRole.Viewer,
            });
        }

        return await DatabaseExceptionHandler.Execute(
                async () => await this.inviteRepository.DeleteAsync(invite.Id));
    }

    private async Task<bool> CheckAccessDataAsync(long inviteId, long expectedTodoListId, string userId, AccessLevel accessLevel)
    {
        var entry = await this.inviteRepository.GetByIdAsync(inviteId);

        if (entry is null || entry.TodoListId != expectedTodoListId)
        {
            throw new AccessDeniedException($"Invite #{inviteId} doesn't belong to the list #{expectedTodoListId}");
        }

        if (accessLevel == AccessLevel.AssignedUser)
        {
            bool isAssigned = entry.UserId == userId;

            if (!isAssigned)
            {
                throw new AccessDeniedException($"User {userId} does not have access to Response on the invite #{inviteId}");
            }

            return true;
        }

        if (!await this.HasAccessAsync(expectedTodoListId, userId, accessLevel))
        {
            throw new AccessDeniedException($"User {userId} does not have access to List #{expectedTodoListId}");
        }

        return true;
    }
}
