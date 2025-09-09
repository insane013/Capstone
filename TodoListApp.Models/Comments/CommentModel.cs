namespace TodoListApp.Models.Comments;
public class CommentModel
{
    public long Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public string CreatedUserId { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; }

    public long TaskId { get; set; }
}
