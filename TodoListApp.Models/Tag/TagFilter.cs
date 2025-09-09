namespace TodoListApp.Models.Tag;
public class TagFilter : BaseFilter
{
    public TagFilter()
    {
        this.PageSize = 30;
    }

    public long FromTodoList { get; set; }

    public bool OnlyAvailable { get; set; }

    public TagSearchOptions? SearchOptions { get; set; }
}
