using System.ComponentModel.DataAnnotations;
using TodoListApp.Models.User;

namespace TodoListApp.Models.WebApp;

/// <summary>
/// Web view model used to show list of users available to be assigned to the task.
/// </summary>
public class ReassignUserWebModel
{
    public long TodoId { get; set; }

    [Required]
    public string CurrentUserId { get; set; } = string.Empty;

    public long TodoListId { get; set; }

    public ICollection<ViewUserInfo> AvailableUsers { get; } = new List<ViewUserInfo>();

    public void SetUsers(ICollection<ViewUserInfo> users)
    {
        this.AvailableUsers.Clear();

        if (users != null && users.Count > 0)
        {
            foreach (ViewUserInfo user in users)
            {
                this.AvailableUsers.Add(user);
            }
        }
    }
}
