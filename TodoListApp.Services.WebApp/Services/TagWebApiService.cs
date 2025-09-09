using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoListApp.Models;
using TodoListApp.Models.Tag;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;

public class TagWebApiService : ITagWebApiService
{
    private readonly HttpClient httpClient;

    public TagWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> DeleteAsync(TagModel model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Tags/{model.Id}?todoListId={model.TodoListId}&tag={model.Tag}";

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

    public async Task<PaginatedResult<TagModel>> GetFromUserAsync(TagFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Tags?pageSize={filter.PageSize}" +
            $"&pageNumber={filter.PageNumber}";

        if (filter.OnlyAvailable)
        {
            url += $"&OnlyAvailable={filter.OnlyAvailable}";
        }

        if (token != null)
        {

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<TagModel>>();
                return data ?? new PaginatedResult<TagModel>();
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                    throw new ApplicationException(error?.Message ?? "Unexpected error.");
                }
            }
        }

        return new PaginatedResult<TagModel>();
    }
}
