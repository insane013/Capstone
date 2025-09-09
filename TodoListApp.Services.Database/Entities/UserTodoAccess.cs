using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Services.Database.Entities;

/// <summary>
/// Database representation of Todolist-User access.
/// </summary>
public class UserTodoAccess
{
    /// <summary>
    /// Access level representation.
    /// </summary>
    public enum TodoRole
    {
        Viewer = 0,
        Editor = 1,
        Owner = 2,
    }

    /// <summary>
    /// Gets or sets id of user.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets id of todolist.
    /// </summary>
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets or sets todolist navigation property.
    /// </summary>
    [ForeignKey("TodoListId")]
    public TodoListEntity TodoList { get; set; } = null!;

    /// <summary>
    /// Gets or sets access level.
    /// </summary>
    public TodoRole Role { get; set; }
}
