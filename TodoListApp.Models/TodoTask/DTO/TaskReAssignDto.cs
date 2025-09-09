using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskReAssignDto
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

    [Required]
    public string OtherUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    public override string ToString()
    {
        return $"Assign task #{this.TodoId} from TodoList #{this.TodoListId} to user with ID = {this.OtherUserId}.";
    }
}
