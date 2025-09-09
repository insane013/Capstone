using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace TodoListApp.Models.Invite.DTO;
public class InviteCreateDto
{
    [Required]
    public IEnumerable<string> Users { get; set; } = Enumerable.Empty<string>();

    [Required]
    public long TodoListId { get; set; }

    public string Message { get; set; } = string.Empty;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"TodoList: {this.TodoListId}");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Message: {this.Message}\n");
        _ = sb.AppendLine(CultureInfo.InvariantCulture, $"Users:");

        foreach (var user in this.Users)
        {
            _ = sb.AppendLine(CultureInfo.InvariantCulture, $"{user}\n");
        }

        return sb.ToString();
    }
}
