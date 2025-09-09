using TodoListApp.Models;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;

namespace TodoListApp.Services;

public interface ICommentDatabaseService
{
    public Task<PaginatedResult<CommentModel>> GetAllAsync(string userId, CommentFilter filter);

    public Task<CommentModel?> GetByIdAsync(string userId, long commentId);

    public Task<CommentModel?> AddAsync(string userId, CommentCreateDto comment);

    public Task<CommentModel?> UpdateAsync(string userId, CommentUpdateDto comment);

    public Task<bool> DeleteAsync(string userId, CommentDeleteDto comment);
}
