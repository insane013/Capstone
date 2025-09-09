using System.Globalization;
using System.Text;
using static TodoListApp.Models.TodoTask.TodoTaskModel;

namespace TodoListApp.Models.TodoTask;

/// <summary>
/// Filter condition for tasks.
/// </summary>
public class TodoTaskFilter : BaseFilter
{
    public enum SortOption
    {
        None,
        TitleAsc,
        TitleDesc,
        DeadlineAsc,
        DeadlineDesc,
    }

    public long TodoListId { get; set; }

    public bool ShowComplete { get; set; } = true;

    public bool ShowOverdue { get; set; } = true;

    public bool ShowPending { get; set; } = true;

    public DateTime? DeadlineBefore { get; set; }

    public DateTime? DeadlineAfter { get; set; }

    public bool OnlyAssigned { get; set; }

    public string? Tag { get; set; } = string.Empty;

    public SortOption SortBy { get; set; } = SortOption.None;

    public IEnumerable<TaskPriority> Priorities { get; set; } = new List<TaskPriority>();

    public TaskSearchOptions? SearchOptions { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tTodoListId: {this.TodoListId}");

        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tShow Completed: {this.ShowComplete}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tShow Overdue: {this.ShowOverdue}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tShow Pending: {this.ShowPending}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tDeadline Before: {this.DeadlineBefore}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tDeadline After: {this.DeadlineAfter}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tOnlyAssignedUser: {this.OnlyAssigned}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tTag: {this.Tag}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tPriorities:");

        foreach (var p in this.Priorities)
        {
            _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\t\tPriority: {p}");
        }

        _ = sb.Append(CultureInfo.InvariantCulture, $"{this.SearchOptions}");

        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tSort By: {this.SortBy}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"\tPageNumber: {this.PageNumber}");

        return sb.ToString();
    }
}
