using TodoListApp.Models;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;
using TodoListApp.Models.WebApp;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface IInviteWebApiService
{
    public Task<PaginatedResult<InviteModel>> GetFromListAsync(InviteFilter filter, string? token);

    public Task<PaginatedResult<InviteModel>> GetFromUserAsync(InviteFilter filter, string? token);

    public Task<InviteModel?> GetAsync(long id, string? token);

    public Task<PaginatedResult<InviteModel>> InviteAsync(InviteUsersWebModel model, string? token);

    public Task<InviteModel?> UpdateAsync(InviteUpdateDto model, string? token);

    public Task<bool> DeleteAsync(InviteDeleteDto model, string? token);

    public Task<bool> ResponseAsync(InviteResponseDto model, string? token);
}
