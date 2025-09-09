namespace TodoListApp.Models.Tag;
public class TagModel
{
    public long Id { get; set; }

    public long TodoListId { get; set; }

    public string TodoListTitle { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"List #{this.TodoListId}, Tag: {this.Tag}";
    }
}
