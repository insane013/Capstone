using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TodoListApp.Models;
using TodoListApp.Services.WebApp.Helpers;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;
public class FilterStorageService : IFilterStorageService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly Dictionary<string, BaseFilter> savedFilters = new Dictionary<string, BaseFilter>();

    public FilterStorageService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public void SaveFilter<T>(string name, T filter) where T : BaseFilter
    {
        var context = this.httpContextAccessor.HttpContext;

        if (context == null)
        {
            return;
        }

        this.savedFilters[name] = filter;

        var json = JsonSerializer.Serialize(filter, JsonSerializationHelper.DefaultOptions);
        context.Response.Cookies.Append(name, json, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1),
        });
    }

    public T? LoadFilter<T>(string name) where T : BaseFilter
    {
        var context = this.httpContextAccessor.HttpContext;

        if (context == null)
        {
            return null;
        }

        if (this.savedFilters.TryGetValue(name, out var filter))
        {
            return (T)filter;
        }

        if (context.Request.Cookies.TryGetValue(name, out var json) && !string.IsNullOrEmpty(json))
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, JsonSerializationHelper.DefaultOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        return null;
    }
}

