using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoList.DTO;

/// <summary>
/// Business logic TodoListModel.
/// </summary>
public class TodoListChangeOwnerDto
{
    [Required]
    public long TodoListId { get; set; }

    [Required]
    public string NewOwnerId { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Changing owner of todoList ID = {this.TodoListId} to {this.NewOwnerId}";
    }
}
