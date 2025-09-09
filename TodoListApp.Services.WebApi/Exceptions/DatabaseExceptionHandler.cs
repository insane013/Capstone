using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Services.WebApi.Exceptions;

internal static class DatabaseExceptionHandler
{
    public static async Task<T> Execute<T>(Func<Task<T>> func, string? duplicateError = null, string? relatedDataNotFoundError = null)
    {
        ArgumentNullException.ThrowIfNull(func);
        try
        {
            return await func();
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 2601) // unique constraint
                {
                    throw new InvalidOperationException(duplicateError ?? "Record with this UNIQUE constraint already exists.");
                }

                if (sqlEx.Number == 547) // foreign key does not exist
                {
                    throw new KeyNotFoundException(relatedDataNotFoundError ?? "Cannot find related data.");
                }

                if (sqlEx.Number == 515) // nullable constraint
                {
                    throw new InvalidOperationException("Passed value cannot be null");
                }
            }

            throw;
        }
    }
}
