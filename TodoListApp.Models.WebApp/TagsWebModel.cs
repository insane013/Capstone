using TodoListApp.Models.Tag;

namespace TodoListApp.Models.WebApp;

/// <summary>
/// Web view model used to show paginated list of tags.
/// </summary>
public class TagsWebModel
{
    public IEnumerable<TagModel> Tags { get; set; } = new List<TagModel>();

    public PagingInfo Pagination { get; set; } = new PagingInfo();

    public long TodoListId { get; set; }

    public string FilterName { get; set; } = string.Empty;

    public TagFilter Filter { get; set; } = new TagFilter();
}
