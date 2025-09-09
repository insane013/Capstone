using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Database.Entities;

/// <summary>
/// Database representation of Invite.
/// </summary>
public class InviteEntity
{
    /// <summary>
    /// Gets or sets Id of invite.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets related todolist id.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets or sets Id of invited user.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets invite message.
    /// </summary>
    public string Message { get; set; } = "We want to invite you to our cool todolist!";

    /// <summary>
    /// Gets or sets Invite's creation time.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets related todolist navigation property.
    /// </summary>
    public TodoListEntity TodoList { get; set; } = null!;
}
