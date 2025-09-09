using System.ComponentModel.DataAnnotations;
using TodoListApp.Models.Tag;
using TodoListApp.Models.TodoTask;

namespace TodoListApp.Models.TodoList;

/// <summary>
/// Business logic TodoListModel.
/// </summary>
public class TodoListModel
{
    /// <summary>
    /// Gets or sets TodoList id.
    /// </summary>
    [Required]
    public long TodoListId { get; set; }

    /// <summary>
    /// Gets or sets TodoList title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets TodoList description.
    /// </summary>
    public string? Description { get; set; }

    [Required]
    public string OwnerId { get; set; } = string.Empty;

    public TodoAccessModel? CurrentUserAccessInfo { get; set; }

    public DateTime CreatedTime { get; set; }

    public int TaskCount { get; set; }

    public int CompletedTaskCount { get; set; }

    public IEnumerable<TagModel> Tags { get; set; } = new List<TagModel>();

    /// <summary>
    /// Gets TodoList task collection.
    /// </summary>
    public ICollection<TodoTaskModel> Tasks { get; } = new List<TodoTaskModel>();

    public void SetTasks(ICollection<TodoTaskModel> tasks)
    {
        this.Tasks.Clear();

        if (tasks != null)
        {
            foreach (var role in tasks)
            {
                this.Tasks.Add(role);
            }
        }
    }

    public override string ToString()
    {
        return $"Id: {this.TodoListId} Title: {this.Title}\nDesc: {this.Description}";
    }
}
