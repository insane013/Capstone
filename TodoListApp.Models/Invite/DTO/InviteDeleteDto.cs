using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.Invite.DTO;
public class InviteDeleteDto
{
    [Required]
    public long Id { get; set; }

    [Required]
    public long TodoListId { get; set; }
}
