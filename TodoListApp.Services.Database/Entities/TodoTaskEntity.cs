using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Services.Database.Entities;

/// <summary>
/// Database representation of TodoTask.
/// </summary>
public class TodoTaskEntity
{
    /// <summary>
    /// Gets or sets TodoTask id.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets TodoTask title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask deadline.
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// Gets or sets Id of user that task assigned for.
    /// </summary>
    public string AssignedUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoTask deadline.
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets TodoTask creation time.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Gets or sets TodoTask update time.
    /// </summary>
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the task si completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets Id of related TodoList.
    /// </summary>
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets or sets related TodoList.
    /// </summary>
    public TodoListEntity TodoList { get; set; } = null!;

    /// <summary>
    /// Gets list of related comments.
    /// </summary>
    public IEnumerable<CommentEntity> Comments { get; } = new List<CommentEntity>();

    /// <summary>
    /// Gets list of related tags.
    /// </summary>
    public ICollection<TaskTagEntity> Tags { get; } = new List<TaskTagEntity>();
}
