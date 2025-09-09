using System.ComponentModel.DataAnnotations;
using TodoListApp.Models.Tag;
using static TodoListApp.Models.TodoTask.TodoTaskModel;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskCreateDto
{
    /// <summary>
    /// Gets or sets TodoTask title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask description.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask deadline.
    /// </summary>
    public DateTime Deadline { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(1);

    public TaskPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets related TodoList id.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    public IEnumerable<TagModel> Tags { get; set; } = new List<TagModel>();

    public override string ToString()
    {
        return $"List: {this.TodoListId} Title: {this.Title}\nDesc: {this.Description}\nDeadline: {this.Deadline}";
    }
}
