using Microsoft.AspNetCore.Mvc;
using TodoListApp.Helpers;
using TodoListApp.Models;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.WebApp.Helpers;

public class FilterHandler : IFilterHandler
{
    private readonly IFilterStorageService filterService;
    private readonly ILogger<FilterHandler> logger;

    public FilterHandler(IFilterStorageService filterService, ILogger<FilterHandler> logger)
    {
        this.filterService = filterService;
        this.logger = logger;
    }

    public IActionResult Apply<TFilter>(Controller controller, TFilter filter, Uri? returnUrl, string filterName)
        where TFilter : BaseFilter
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(controller);

        LoggingDelegates.LogInfo(this.logger, $"Applying filter:\n{filter}", null);

        this.filterService.SaveFilter(filterName, filter);

        if (returnUrl is null)
        {
            return controller.RedirectToAction("AssignedTasks", "TodoTask");
        }

        return controller.Redirect(returnUrl.ToString());
    }

    public TFilter? Load<TFilter>(string filterName)
        where TFilter : BaseFilter
    {
        return this.filterService.LoadFilter<TFilter>(filterName);
    }
}
