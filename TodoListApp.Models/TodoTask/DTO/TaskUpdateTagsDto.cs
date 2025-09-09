using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoTask.DTO;

/// <summary>
/// DTO for task creation.
/// </summary>
public class TaskUpdateTagsDto
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Required]
    public long TodoId { get; set; }

    [Required]
    public long TodoListId { get; set; }

    public IEnumerable<string> Tags { get; set; } = new List<string>();
}
