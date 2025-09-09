namespace TodoListApp.Models.TodoTask;
public class TaskSearchOptions
{
    public string? Title { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? Deadline { get; set; }

    public override string ToString()
    {
        return $"\tSearch for:\n\t\tTitle contains: {this.Title}\n\t\tCreated on: {this.CreatedDate}\n\t\tDue on: {this.Deadline}\n";
    }
}
