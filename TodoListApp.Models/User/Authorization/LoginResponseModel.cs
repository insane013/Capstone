namespace TodoListApp.Models.User.Authorization;

/// <summary>
/// Model to respond on login aptempt.
/// </summary>
public class LoginResponseModel
{
    /// <summary>
    /// Gets or sets JWT token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
