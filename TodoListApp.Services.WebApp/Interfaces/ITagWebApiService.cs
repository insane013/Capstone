using TodoListApp.Models;
using TodoListApp.Models.Tag;

namespace TodoListApp.Services.WebApp.Interfaces;

/// <summary>
/// Interface with TodoList service functionality.
/// </summary>
public interface ITagWebApiService
{
    public Task<PaginatedResult<TagModel>> GetFromUserAsync(TagFilter filter, string? token);

    public Task<bool> DeleteAsync(TagModel model, string? token);
}
