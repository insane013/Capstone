using Microsoft.AspNetCore.Identity;

namespace TodoListApp.Services.Database.Users.Identity;

/// <summary>
/// Database representation of User.
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Gets or sets User display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets User unique tag.
    /// </summary>
    public string UniqueTag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets User registration time.
    /// </summary>
    public DateTime RegistrationTime { get; set; }
}
