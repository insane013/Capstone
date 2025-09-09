using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.Invite.DTO;
public class InviteUpdateDto
{
    [Required]
    public long Id { get; set; }

    [Required]
    public long TodoListId { get; set; }

    public string Message { get; set; } = "We want to invite you to our cool todolist!";
}
