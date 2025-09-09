using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Models;
using TodoListApp.Models.User;
using TodoListApp.Services.Database.Users.Identity;
using TodoListApp.Services.WebApi.Exceptions;
using TodoListApp.Services.WebApi.Mapper;

namespace TodoListApp.Services.WebApi.Users;

/// <summary>
/// TodoList service implementing ITodoListDatabaseService interface.
/// </summary>
public class UserDatabaseService : SecureDatabaseService, IUserDatabaseService
{
    private readonly ITodoAccessDatabaseService todoAccessService;
    private readonly UserManager<User> userManager;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDatabaseService"/> class.
    /// </summary>
    /// <param name="todoListRep">Todolist repository instance.</param>
    public UserDatabaseService(ITodoAccessDatabaseService todoAccessService, ITodoAccessDatabaseService accessService, UserManager<User> userManager, IMapper mapper)
        : base(accessService)
    {
        this.todoAccessService = todoAccessService;
        this.userManager = userManager;
        this.mapper = mapper;
    }

    public async Task<PaginatedResult<ViewUserInfo>> GetAllAsync(string userId, UserFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(userId);

        if (!await this.HasAccessAsync(filter.TodoListId, userId))
        {
            throw new AccessDeniedException($"User {userId} does not have access to TodoList {filter.TodoListId}");
        }

        var query = this.userManager.Users;
        var accesses = await this.todoAccessService.GetFromTodoListAsync(filter.TodoListId);

        var accessUserIds = accesses.Select(a => a.UserId).ToList();
        var users = await query.Where(x => accessUserIds.Contains(x.Id)).ToListAsync();

        if (filter.Roles != null && filter.Roles.Count > 0)
        {
            users = users.Where(x =>
            {
                var access = accesses.FirstOrDefault(y => y.UserId == x.Id);
                return access != null && filter.Roles.Any(role => access.Role == role);
            }).ToList();
        }

        var result = PaginationHelper.Paginate(
            users,
            filter.PageNumber,
            filter.PageSize,
            x =>
            {
                return this.mapper.Map<ViewUserInfo>(x);
            });

        return result;
    }

    public async Task<ViewUserInfo?> GetByTagAsync(string tag)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.UniqueTag == tag);

        return user is not null ? this.mapper.Map<ViewUserInfo>(user) : null;
    }

    public async Task<ViewUserInfo?> GetByEmailAsync(string email)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.Email == email);

        return user is not null ? this.mapper.Map<ViewUserInfo>(user) : null;
    }

    public async Task<ViewUserInfo?> GetByIdAsync(string userId)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        return user is not null ? this.mapper.Map<ViewUserInfo>(user) : null;
    }

    public async Task<string> GetTagByIdAsync(string userId)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        return user is not null ? user.UniqueTag : string.Empty;
    }

    public async Task<string> GetIdByTagAsync(string tag)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.UniqueTag == tag);

        return user is not null ? user.Id : string.Empty;
    }

    public async Task<ViewUserInfo?> ChangeTag(string userId, string newTag)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        ArgumentNullException.ThrowIfNull(user);

        bool newTagUserExist = (await this.GetByTagAsync(newTag)) != null;

        if (newTagUserExist)
        {
            throw new InvalidOperationException($"User with tag {newTag} already exists.");
        }

        user.UniqueTag = newTag;

        _ = await this.userManager.UpdateAsync(user);

        return this.mapper.Map<ViewUserInfo>(user);
    }
}
