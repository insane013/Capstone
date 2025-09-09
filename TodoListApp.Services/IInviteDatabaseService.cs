using TodoListApp.Models;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;

namespace TodoListApp.Services;

public interface IInviteDatabaseService
{
    public Task<PaginatedResult<InviteModel>> GetFromListAsync(string userId, InviteFilter filter);

    public Task<PaginatedResult<InviteModel>> GetFromUserAsync(string userId, InviteFilter filter);

    public Task<InviteModel?> GetByIdAsync(string userId, long inviteId);

    public Task<IEnumerable<InviteModel>> AddAsync(string userId, InviteCreateDto invite);

    public Task<InviteModel?> UpdateAsync(string userId, InviteUpdateDto invite);

    public Task<bool> DeleteAsync(string userId, InviteDeleteDto invite);

    public Task<bool> UserResponseAsync(string userId, InviteResponseDto invite);
}
