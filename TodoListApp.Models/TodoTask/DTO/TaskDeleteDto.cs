using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskDeleteDto
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

    [Required]
    public long TodoListId { get; set; }

    public override string ToString()
    {
        return $"Deleting task #{this.TodoId} from list #{this.TodoListId}";
    }
}
