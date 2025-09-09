namespace TodoListApp.Models.Comments;
public class CommentFilter : BaseFilter
{
    public long TaskId { get; set; }

    public override string ToString()
    {
        return $"TaskId: {this.TaskId}";
    }
}
