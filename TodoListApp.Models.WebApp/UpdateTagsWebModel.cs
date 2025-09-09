using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.WebApp;

/// <summary>
/// Web view model used to pass tag information from Form to WebApp controller.
/// </summary>
public class UpdateTagsWebModel
{
    [Required]
    public long TaskId { get; set; }

    [Required]
    public long TodoListId { get; set; }

    public string? Tags { get; set; } = string.Empty;

    public IEnumerable<string> GetTagList()
    {
        if (string.IsNullOrEmpty(this.Tags))
        {
            return Enumerable.Empty<string>();
        }

        return this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
