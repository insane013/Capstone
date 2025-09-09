using TodoListApp.Models.Comments;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.Models.WebApp;

public class CommentsFromTaskWebModel
{
    public long TaskId { get; set; }

    public PagingInfo Pagination { get; set; } = new PagingInfo();

    public TodoRole AccessLevel { get; set; } = TodoRole.Viewer;

    public string FilterName { get; set; } = "CommentFilter";

    public CommentFilter Filter { get; set; } = new CommentFilter();

    public IEnumerable<CommentModel> Comments { get; set; } = Enumerable.Empty<CommentModel>();
}
