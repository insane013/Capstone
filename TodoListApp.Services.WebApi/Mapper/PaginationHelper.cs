using TodoListApp.Models;

namespace TodoListApp.Services.WebApi.Mapper;
public static class PaginationHelper
{
    public static PaginatedResult<TOut> Paginate<TIn, TOut>(
        IEnumerable<TIn> source,
        int pageNumber,
        int pageSize,
        Func<TIn, TOut> map)
    {
        var materailizedSource = source.ToList();

        var count = materailizedSource.Count;

        var paged = materailizedSource
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(map)
            .ToList();

        return new PaginatedResult<TOut>
        {
            TotalItems = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            PageCount = (int)Math.Ceiling(count / (double)pageSize),
            Items = paged,
        };
    }
}
