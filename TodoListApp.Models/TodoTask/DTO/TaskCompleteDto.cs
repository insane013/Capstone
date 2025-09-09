using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskCompleteDto
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

    public bool Status { get; set; } = true;

    [Required]
    public long TodoListId { get; set; }

    public override string ToString()
    {
        var status = this.Status ? "completed" : "not completed";

        return $"Mark task with ID: {this.TodoId} as {status}";
    }
}
