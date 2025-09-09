using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Helpers;
using TodoListApp.Services.WebApi.Exceptions;

namespace TodoListApp.WebApi.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ApiExceptionMiddleware> logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null)
        {
            return;
        }

        try
        {
            await this.next(context);
        }
        catch (AccessDeniedException ex)
        {
            LoggingDelegates.LogWarn(this.logger, "Access denied", ex);
            await WriteErrorResponse(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            LoggingDelegates.LogError(this.logger, ex.Message, ex);
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            LoggingDelegates.LogError(this.logger, ex.Message, ex);
            await WriteErrorResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            LoggingDelegates.LogWarn(this.logger, ex.Message, ex);
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2601: // Unique constraint violation
                        LoggingDelegates.LogWarn(this.logger, "Duplicate entry detected.", null);
                        await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "This entry already exists.");
                        break;
                    case 547: // Foreign key violation
                        LoggingDelegates.LogError(this.logger, "Foreign key violation.", null);
                        await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Related data not found.");
                        break;
                    case 515: // Nullable constraint violation
                        LoggingDelegates.LogError(this.logger, "Null value violation.", null);
                        await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Required value cannot be null.");
                        break;
                    default:
                        LoggingDelegates.LogError(this.logger, "Database error.", sqlEx);
                        await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, "Database error occurred.");
                        break;
                }
            }
            else
            {
                LoggingDelegates.LogError(this.logger, "Database error.", dbEx);
                await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
        catch (Exception ex) when (ex is not DbUpdateException)
        {
            LoggingDelegates.LogWarn(this.logger, "Unexcepted exception", ex);
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new { Message = message };
        await context.Response.WriteAsJsonAsync(response);
    }
}
