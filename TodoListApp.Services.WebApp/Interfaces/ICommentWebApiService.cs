using TodoListApp.Models;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface ICommentWebApiService
{
    public Task<PaginatedResult<CommentModel>> GetAllAsync(CommentFilter filter, string? token);

    public Task<CommentModel?> GetAsync(long id, string? token);

    public Task<CommentModel?> AddAsync(CommentCreateDto model, string? token);

    public Task<CommentModel?> UpdateAsync(CommentUpdateDto model, string? token);

    public Task<bool> DeleteAsync(CommentDeleteDto model, string? token);
}
