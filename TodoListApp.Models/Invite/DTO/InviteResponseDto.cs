using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.Invite.DTO;
public class InviteResponseDto
{
    [Required]
    public long Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public long TodoListId { get; set; }

    [Required]
    public bool Accepted { get; set; } = false;
}
