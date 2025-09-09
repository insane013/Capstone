using TodoListApp.Models.User;

namespace TodoListApp.Models.WebApp;

/// <summary>
/// Web view model used to show list of users from certain todolist.
/// </summary>
public class UsersFromTodoListWebModel
{
    public UsersFromTodoListWebModel(long todoListId, ICollection<ViewUserInfo> users)
    {
        this.TodoListId = todoListId;
        this.Users = users;
    }

    public long TodoListId { get; set; }

    public ICollection<ViewUserInfo> Users { get; }
}
