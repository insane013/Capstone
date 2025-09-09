using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Database.Entities;

/// <summary>
/// Database representation of TodoList.
/// </summary>
public class TodoListEntity
{
    [Key]
    /// <summary>
    /// Gets or sets TodoList id.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets TodoList title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoList description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets owner user id.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets todolist creation time.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// Gets collection of related TodoTasks.
    /// </summary>
    public ICollection<TodoTaskEntity> TodoTasks { get; } = new List<TodoTaskEntity>();

    /// <summary>
    /// Gets collection of all user relations.
    /// </summary>
    public ICollection<UserTodoAccess> TodoAccesses { get; } = new List<UserTodoAccess>();

    /// <summary>
    /// Gets list of related invites.
    /// </summary>
    public ICollection<InviteEntity> Invites { get; } = new List<InviteEntity>();

    /// <summary>
    /// Gets list of tags that tasks in this list have.
    /// </summary>
    public ICollection<TaskTagEntity> Tags { get; } = new List<TaskTagEntity>();
}
