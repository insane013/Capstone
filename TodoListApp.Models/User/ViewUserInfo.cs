using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.User;

/// <summary>
/// Model with user information.
/// </summary>
public class ViewUserInfo
{
    /// <summary>
    /// Gets or sets User Id.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user name.
    /// </summary>
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user's unique tag.
    /// </summary>
    [Required]
    public string UniqueTag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user's email address.
    /// </summary>
    [Required, EmailAddress]
    public string? Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user's registration time and date.
    /// </summary>
    public DateTime? RegistrationTime { get; set; }
}
