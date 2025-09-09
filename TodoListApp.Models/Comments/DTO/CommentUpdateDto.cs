using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.Comments.DTO;
public class CommentUpdateDto
{
    public long Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public string CreatedUserId { get; set; } = string.Empty;

    [Required]
    public long TaskId { get; set; }
}
