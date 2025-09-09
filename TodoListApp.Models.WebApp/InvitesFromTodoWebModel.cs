using TodoListApp.Models.Invite;

namespace TodoListApp.Models.WebApp;
public class InvitesFromTodoWebModel
{
    public long TodoListId { get; set; }

    public IEnumerable<InviteModel> Invites { get; set; } = new List<InviteModel>();
}
