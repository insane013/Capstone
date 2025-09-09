namespace TodoListApp.Services.Database.Entities;

/// <summary>
/// Database representation of tag.
/// </summary>
public class TaskTagEntity
{
    /// <summary>
    /// Gets or sets Id of tag.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets related todolist Id.
    /// </summary>
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets or sets tag.
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets related todolist navigation property.
    /// </summary>
    public TodoListEntity TodoList { get; set; } = null!;

    /// <summary>
    /// Gets Id list of associated tasks.
    /// </summary>
    public ICollection<TodoTaskEntity> RelatedTasks { get; } = new List<TodoTaskEntity>();
}
