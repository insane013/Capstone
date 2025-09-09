using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models;

namespace TodoListApp.WebApp.Helpers;

public interface IFilterHandler
{
    IActionResult Apply<TFilter>(Controller controller, TFilter filter, Uri? returnUrl, string filterName)
        where TFilter : BaseFilter;

    TFilter? Load<TFilter>(string filterName)
        where TFilter : BaseFilter;
}
