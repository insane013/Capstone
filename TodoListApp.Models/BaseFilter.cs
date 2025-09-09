using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models;

/// <summary>
/// Base class for all filters.
/// </summary>
public class BaseFilter
{
    /// <summary>
    /// Gets or sets Page size for pagination.
    /// </summary>
    [Required, Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0.")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets number of items on sinngle page for pagination.
    /// </summary>
    [Required, Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0.")]
    public int PageNumber { get; set; } = 1;
}
