using TodoListApp.Models.TodoList;

namespace TodoListApp.Models.WebApp;

public class AllTodoListsWebModel
{
    public string FilterName { get; set; } = string.Empty;

    public TodoListFilter Filter { get; set; } = new TodoListFilter();

    public PagingInfo Pagination { get; set; } = new PagingInfo();

    public IEnumerable<TodoListModel> TodoLists { get; set; } = new List<TodoListModel>();
}
