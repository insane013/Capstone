using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.User;

/// <summary>
/// DTO for updating user info.
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// Gets or sets id of user to update.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets new user name.
    /// </summary>
    [Required, MinLength(2, ErrorMessage = "Minmum length is 2")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets new user tag.
    /// </summary>
    public string UniqueTag { get; set; } = string.Empty;
}
