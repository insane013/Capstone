using TodoListApp.Models.TodoTask;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.Models.WebApp;

/// <summary>
/// Web view model used to show paginated task list.
/// </summary>
public class TaskListWebModel
{
    public long? TodoListId { get; set; }

    public string FilterName { get; set; } = string.Empty;

    public TodoTaskFilter Filter { get; set; } = new TodoTaskFilter();

    public PagingInfo Pagination { get; set; } = new PagingInfo();

    public TodoRole AccessLevel { get; set; } = TodoRole.Viewer;

    public IEnumerable<TodoTaskModel> TodoTasks { get; set; } = new List<TodoTaskModel>();
}
