namespace TodoListApp.Models.Comments.DTO;
public class CommentCreateDto
{
    public string Content { get; set; } = string.Empty;

    public long TaskId { get; set; }

    public override string ToString()
    {
        return $"TaskID: {this.TaskId}\nContent: {this.Content}";
    }
}
