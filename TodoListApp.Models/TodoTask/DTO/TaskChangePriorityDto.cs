using System.ComponentModel.DataAnnotations;
using static TodoListApp.Models.TodoTask.TodoTaskModel;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskChangePriorityDto
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

    [Required]
    public TaskPriority NewPriority { get; set; } = TaskPriority.Standart;

    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    public override string ToString()
    {
        return $"Changing Task #{this.TodoId} from TodoList #{this.TodoListId}  priority to {this.NewPriority}.";
    }
}
