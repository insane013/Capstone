using System.ComponentModel.DataAnnotations;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.Models.User;

/// <summary>
/// Filter used to filtering users.
/// </summary>
public class UserFilter : BaseFilter
{
    /// <summary>
    /// Gets or sets Todolist Id to get users from.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets a collection of roles to filter user.
    /// </summary>
    public ICollection<TodoRole> Roles { get; } = new List<TodoRole>();

    /// <summary>
    /// Set a roles to filter users.
    /// </summary>
    /// <param name="roles">Collection of TodoRole enum.</param>
    public void SetRoles(ICollection<TodoRole> roles)
    {
        this.Roles.Clear();

        if (roles != null)
        {
            foreach (var role in roles)
            {
                this.Roles.Add(role);
            }
        }
    }
}
