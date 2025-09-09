namespace TodoListApp.Models.Invite;
public class InviteFilter : BaseFilter
{
    public string ForUser { get; set; } = string.Empty;

    public long ForTodoList { get; set; }
}
