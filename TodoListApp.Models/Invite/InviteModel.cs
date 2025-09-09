using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.Invite;
public class InviteModel
{
    [Required]
    public long Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public long TodoListId { get; set; }

    public string TodoListTitle { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Message { get; set; } = string.Empty;
}
