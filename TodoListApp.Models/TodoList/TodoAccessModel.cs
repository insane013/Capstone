using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoList;
public class TodoAccessModel
{
    public enum TodoRole
    {
        Viewer,
        Editor,
        Owner,
    }

    [Required]
    public long TodoId { get; set; }

    [Required]
    required public string UserId { get; set; }

    public TodoRole Role { get; set; } = TodoRole.Viewer;
}
