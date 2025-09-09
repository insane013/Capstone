using TodoListApp.Models.TodoTask;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.Models.WebApp;

/// <summary>
/// Web view model used to show TodoTask page with certain access level.
/// </summary>
public class TodoTaskWebModel
{
    public TodoTaskModel TaskModel { get; set; } = null!;

    public TodoRole AccessLevel { get; set; } = TodoRole.Viewer;
}
