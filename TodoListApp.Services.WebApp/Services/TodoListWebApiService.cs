using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoListApp.Models;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;
public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;

    public TodoListWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<TodoListModel?> AddNewAsync(TodoListCreateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoLists/";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoListModel>();
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

    public async Task<bool> DeleteAsync(long id, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoLists/{id}";

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

    public async Task<PaginatedResult<TodoListModel>> GetAllAsync(TodoListFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoLists?pageSize={filter.PageSize}&pageNumber={filter.PageNumber}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<TodoListModel>>();
                return data ?? new PaginatedResult<TodoListModel>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return new PaginatedResult<TodoListModel>();
    }

    public async Task<TodoListModel?> GetOneAsync(long id, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoLists/{id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoListModel>();
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

    public async Task<TodoListModel?> UpdateAsync(TodoListUpdateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}TodoLists/{model.TodoListId}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TodoListModel>();
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
