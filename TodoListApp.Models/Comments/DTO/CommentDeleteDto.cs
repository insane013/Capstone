namespace TodoListApp.Models.Comments.DTO;
public class CommentDeleteDto
{
    public long Id { get; set; }

    public string CreatedUserId { get; set; } = string.Empty;

    public long TaskId { get; set; }
}
