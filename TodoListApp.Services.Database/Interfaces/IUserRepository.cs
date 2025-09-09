using TodoListApp.Services.Database.Users.Identity;

namespace TodoListApp.Services.Database.Interfaces;

/// <summary>
/// Interface for CRUD operations related to users in the TodoList application.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Asynchronously retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user entity, or <c>null</c> if not found.</returns>
    public Task<User?> GetByIdAsync(string id);

    /// <summary>
    /// Asynchronously retrieves a user by their email identifier.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <returns>The user entity, or <c>null</c> if not found.</returns>
    public Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Asynchronously retrieves a user by their login.
    /// </summary>
    /// <param name="login">The login (username) of the user.</param>
    /// <returns>The user entity, or <c>null</c> if not found.</returns>
    public Task<User?> GetByLoginAsync(string login);
}
