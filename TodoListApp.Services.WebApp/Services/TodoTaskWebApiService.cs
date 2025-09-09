using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Helpers;
using TodoListApp.Models;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;
public class TodoTaskWebApiService : ITodoTaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<TodoTaskWebApiService> logger;

    public TodoTaskWebApiService(HttpClient httpClient, ILogger<TodoTaskWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<TodoTaskModel?> AddNewAsync(TaskCreateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/";


        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoTaskModel>();
                return data;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return null;
    }

    public async Task<bool> DeleteAsync(TaskDeleteDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{model.TodoId}?todoListId={model.TodoListId}&todoId={model.TodoId}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return false;
    }

    public async Task<PaginatedResult<TodoTaskModel>> GetAllAsync(TodoTaskFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks?pageSize={filter.PageSize}" +
              $"&pageNumber={filter.PageNumber}" +
              $"&TodoListId={filter.TodoListId}" +
              $"&ShowComplete={filter.ShowComplete}" +
              $"&ShowOverdue={filter.ShowOverdue}" +
              $"&ShowPending={filter.ShowPending}" +
              $"&DeadlineBefore={Uri.EscapeDataString(filter.DeadlineBefore?.ToString("yyyy-MM-ddThh:mm") ?? string.Empty)}" +
              $"&DeadlineAfter={Uri.EscapeDataString(filter.DeadlineAfter?.ToString("yyyy-MM-ddThh:mm") ?? string.Empty)}" +
              $"&OnlyAssigned={filter.OnlyAssigned}" +
              $"&Tag={filter.Tag}";

        foreach (var p in filter.Priorities)
        {
            url += $"&Priorities={Uri.EscapeDataString(p.ToString())}";
        }

        url += $"&SearchOptions.Title={Uri.EscapeDataString(filter.SearchOptions?.Title ?? string.Empty)}";
        url += $"&SearchOptions.CreatedDate={Uri.EscapeDataString(filter.SearchOptions?.CreatedDate?.ToString("yyyy-MM-dd") ?? string.Empty)}";
        url += $"&SearchOptions.Deadline={Uri.EscapeDataString(filter.SearchOptions?.Deadline?.ToString("yyyy-MM-dd") ?? string.Empty)}";
        url += $"&SortBy={Uri.EscapeDataString(filter.SortBy.ToString())}";

        LoggingDelegates.LogInfo(this.logger, $"Request on {url}", null);

        if (token != null)
        {

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<TodoTaskModel>>();
                return data ?? new PaginatedResult<TodoTaskModel>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return new PaginatedResult<TodoTaskModel>();
    }

    public async Task<TodoTaskModel?> GetOneAsync(long id, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoTaskModel>();
                return data;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return null;
    }

    public async Task<TodoTaskModel?> UpdateAsync(TaskUpdateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{model.TodoId}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoTaskModel>();
                return data;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return null;
    }

    public async Task<bool> ChangeStateAsync(TaskCompleteDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{model.TodoId}/Status";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return false;
    }

    public async Task<bool> ChangePriorityAsync(TaskChangePriorityDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{model.TodoId}/Priority";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return false;
    }

    public async Task<bool> ChangeAssignedUserAsync(TaskReAssignDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{model.TodoId}/Assigned";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return false;
    }

    public async Task<TodoTaskModel?> UpdateTagsAsync(UpdateTagsWebModel model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoTasks/{model.TaskId}/Tags";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dtoModel = new TaskUpdateTagsDto
            {
                TodoId = model.TaskId,
                TodoListId = model.TodoListId,
                Tags = model.GetTagList(),
            };

            var content = JsonContent.Create(dtoModel);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoTaskModel?>();
                return data;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return null;
    }
}
