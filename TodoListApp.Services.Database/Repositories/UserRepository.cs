using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.Database.Users;
using TodoListApp.Services.Database.Users.Identity;

namespace TodoListApp.Services.Database.Repositories;

/// <summary>
/// Repository for managing User entities.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">Database context instance.</param>
    public UserRepository(UserDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public async Task<User?> GetByIdAsync(string id)
    {
        return await this.context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Retrieves a user by their login (username) asynchronously.
    /// </summary>
    /// <param name="login">The user's login name.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public async Task<User?> GetByLoginAsync(string login)
    {
        return await this.context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == login);
    }

    /// <summary>
    /// Retrieves a user by their email address asynchronously.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await this.context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);
    }
}
