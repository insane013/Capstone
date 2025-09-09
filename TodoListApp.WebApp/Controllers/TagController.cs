using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models.Tag;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;
using TodoListApp.WebApp.Helpers;

namespace TodoListApp.WebApp.Controllers;

[Route("Tags")]
[Authorize]
public class TagController : BaseController
{
    private const string TagFilterName = "AvailableTags";

    private readonly ITagWebApiService tagService;
    private readonly IFilterHandler filterHandler;

    public TagController(ITagWebApiService tagService, IFilterHandler filterHandler, ILogger<TagController> logger)
        : base(logger)
    {
        this.tagService = tagService;
        this.filterHandler = filterHandler;
    }

    [HttpPost]
    [Route("ApplyFilterJson")]
    public IActionResult ApplyFilterJson([FromBody] TodoTaskFilter filter, [FromQuery] Uri? returnUrl, [FromQuery] string filterName = TagFilterName)
    {
        return this.filterHandler.Apply(this, filter, returnUrl, filterName);
    }

    [HttpPost]
    [Route("ApplySearch")]
    public IActionResult ApplySearch(TagSearchOptions searchOpt, Uri? returnUrl, string filterName = TagFilterName)
    {
        ArgumentNullException.ThrowIfNull(searchOpt);

        LoggingDelegates.LogInfo(this.Logger, $"Applying search:\n{searchOpt}", null);

        var currentFilter = this.filterHandler.Load<TagFilter>(filterName) ?? new TagFilter();

        LoggingDelegates.LogInfo(this.Logger, $"Current filter {filterName}:\n{currentFilter}", null);

        currentFilter.SearchOptions = searchOpt;

        return this.filterHandler.Apply(this, currentFilter, returnUrl, filterName);
    }

    [HttpGet]
    [Route("Available")]
    public async Task<IActionResult> AvailableTags()
    {
        var filter = this.filterHandler.Load<TagFilter>(TagFilterName) ?? new TagFilter { OnlyAvailable = true };

        try
        {
            var data = await this.tagService.GetFromUserAsync(filter, this.Token!);

            var viewModel = new TagsWebModel
            {
                Filter = filter,
                FilterName = TagFilterName,
                Pagination = new PagingInfo
                {
                    CurrentPage = data.CurrentPage,
                    PageSize = data.PageSize,
                    TotalPages = data.PageCount,
                },
                Tags = data.Items,
            };

            return this.View(viewModel);
        }
        catch (ApplicationException ex)
        {
            this.TempData["ErrorMessage"] = ex.Message;
            return this.RedirectToAction("AllTodoLists", controllerName: "TodoList");
        }
    }
}
