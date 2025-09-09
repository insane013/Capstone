using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Services.Database.Entities;

/// <summary>
/// Database representation of Comment.
/// </summary>
public class CommentEntity
{
    [Key]
    /// <summary>
    /// Gets or sets Comment Id.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets comment content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Id of user created this comment.
    /// </summary>
    public string CreatedUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Time comment was created.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Gets or sets Id of TodoTask this comment related to.
    /// </summary>
    public long TodoTaskId { get; set; }

    /// <summary>
    /// Gets or sets Navigation property to related task.
    /// </summary>
    [ForeignKey("TodoTaskId")]
    public TodoTaskEntity? TodoTask { get; set; }
}
