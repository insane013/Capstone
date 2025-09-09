using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.User.Authorization;

/// <summary>
/// Model for data used on user loging in.
/// </summary>
public class LoginUserModel
{
    /// <summary>
    /// Gets or sets user login.
    /// </summary>
    [Required, MinLength(2, ErrorMessage = "Minmum length is 2")]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user password.
    /// </summary>
    [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minmum length is 4")]
    public string Password { get; set; } = string.Empty;
}
