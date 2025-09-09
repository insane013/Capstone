using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.TodoList.DTO;

/// <summary>
/// DTO for creating new TodoList. When Mapping you should set OwnerId to entity by yourself from Httpcontext.
/// </summary>
public class TodoListCreateDto
{
    /// <summary>
    /// Gets or sets TodoList title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoList description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Converts DTO data to string format.
    /// </summary>
    /// <returns>String with todoList creation data.</returns>
    public override string ToString()
    {
        return $"Title: {this.Title}\nDesc: {this.Description}";
    }
}
