using System.ComponentModel.DataAnnotations;
using TodoListApp.Models.Tag;
using TodoListApp.Models.TodoList;

namespace TodoListApp.Models.TodoTask;

/// <summary>
/// Business logic TodoTaskModel.
/// </summary>
public class TodoTaskModel
{
    public enum TaskPriority
    {
        Low,
        Standart,
        High,
        Critical,
    }

    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

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
    /// Gets or sets TodoTask creation time.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Gets or sets TodoTask last update time.
    /// </summary>
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// Gets or sets TodoTask deadline.
    /// </summary>
    public DateTime Deadline { get; set; }

    public string AssignedUserId { get; set; } = string.Empty;

    public TodoAccessModel? CurrentUserAccessInfo { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Standart;

    /// <summary>
    /// Gets or sets a value indicating whether the task is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets a value indicating whether the task is completed.
    /// </summary>
    public bool IsOverdue => !this.IsCompleted && this.Deadline < DateTime.UtcNow;

    public IEnumerable<TagModel> Tags { get; set; } = new List<TagModel>();

    public IEnumerable<TagModel> AvailableTags { get; set; } = new List<TagModel>();

    /// <summary>
    /// Gets or sets related TodoList id.
    /// </summary>
    public long TodoListId { get; set; }

    public override string ToString()
    {
        return $"Id: {this.TodoId} Title: {this.Title}\nDesc: {this.Description}\n" +
            $"Priority: {this.Priority}\nCompleted:{this.IsCompleted}";
    }
}
