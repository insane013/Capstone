using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.User.Authorization;

/// <summary>
/// Model of data used on new user registration.
/// </summary>
public class RegisterUserModel
{
    /// <summary>
    /// Gets or sets user email.
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user name.
    /// </summary>
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user tag.
    /// </summary>
    [Required, MinLength(3, ErrorMessage = "MinLength for tag is 3")]
    public string UniqueTag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user password.
    /// </summary>
    [DataType(DataType.Password), Required, MinLength(6, ErrorMessage = "Minimum length is 6")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets confirmation user password. It must be the same as password.
    /// </summary>
    [DataType(DataType.Password), Required, Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
