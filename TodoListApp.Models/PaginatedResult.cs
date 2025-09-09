namespace TodoListApp.Models;

/// <summary>
/// Pagination model with all information.
/// </summary>
/// <typeparam name="T">Model type.</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Gets or sets count of all items in the result.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets number of items on single page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets number of pages.
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Gets or sets current page.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets enumerable with resulted items.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}
