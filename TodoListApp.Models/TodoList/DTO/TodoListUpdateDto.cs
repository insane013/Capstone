using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoList.DTO;

/// <summary>
/// Business logic TodoListModel.
/// </summary>
public class TodoListUpdateDto
{
    [Required]
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets or sets TodoList title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoList description.
    /// </summary>
    public string? Description { get; set; }

    public override string ToString()
    {
        return $"Update todo List:\nID = {this.TodoListId} Title: {this.Title}\nDesc: {this.Description}";
    }
}
