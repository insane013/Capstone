using System.ComponentModel.DataAnnotations;
using static TodoListApp.Models.TodoTask.TodoTaskModel;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskUpdateDto
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

    /// <summary>
    /// Gets or sets TodoTask title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask description.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask deadline.
    /// </summary>
    public DateTime Deadline { get; set; }

    public TaskPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    public override string ToString()
    {
        return $"ID: {this.TodoId} Title: {this.Title}\nDesc: {this.Description}\nDeadline: {this.Deadline}";
    }
}
