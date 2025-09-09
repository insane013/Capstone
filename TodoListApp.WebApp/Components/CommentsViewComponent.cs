using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models;
using TodoListApp.Models.Comments;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Helpers;
using TodoListApp.Services.WebApp.Interfaces;
using static TodoListApp.Models.TodoList.TodoAccessModel;

namespace TodoListApp.WebApp.Components;

public class CommentsViewComponent : ViewComponent
{
    private readonly ICommentWebApiService commentService;
    private readonly ILogger<CommentsViewComponent> logger;
    private readonly IFilterStorageService filterService;

    public CommentsViewComponent(ICommentWebApiService commentService, IFilterStorageService filterService, ILogger<CommentsViewComponent> logger)
    {
        this.commentService = commentService;
        this.filterService = filterService;
        this.logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(string jsonFilter, long taskId, string filterName = "CommentFilter", TodoRole accessLevel = TodoRole.Viewer)
    {
        var filter = JsonSerializer.Deserialize<CommentFilter>(jsonFilter, JsonSerializationHelper.DefaultOptions);

        ArgumentNullException.ThrowIfNull(filter);

        filter.TaskId = taskId;

        this.filterService.SaveFilter(filterName, filter);

        var token = this.Request.Cookies["access_token"];

        LoggingDelegates.LogInfo(this.logger, $"Comments view component call. Filter:\n{filter}", null);

        PaginatedResult<CommentModel> data = new PaginatedResult<CommentModel>();

        try
        {
            data = await this.commentService.GetAllAsync(filter, token!);
        }
        catch (ApplicationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
        }

        return this.View("_AllCommentsPartial", new CommentsFromTaskWebModel
        {
            TaskId = filter.TaskId,
            Comments = data.Items,
            Filter = filter,
            FilterName = filterName,
            Pagination = new PagingInfo
            {
                CurrentPage = data.CurrentPage,
                PageSize = data.PageSize,
                TotalPages = data.PageCount,
            },
            AccessLevel = accessLevel,
        });
    }
}
