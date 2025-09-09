using TodoListApp.Models;
using TodoListApp.Models.Tag;

namespace TodoListApp.Services;

public interface ITagDatabaseService
{

    public Task<PaginatedResult<TagModel>> GetAllAsync(string userId, TagFilter filter);

    public Task<TagModel?> GetByIdAsync(string userId, long tagId);

    public Task<TagModel?> GetAsync(string userId, long todoListId, string tag);

    public Task<TagModel?> AddAsync(string userId, TagModel tag);

    public Task<TagModel?> UpdateAsync(string userId, TagModel tag);

    public Task<bool> DeleteAsync(string userId, TagModel tag);
}
