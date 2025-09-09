using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoListApp.Models;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;

public class CommentWebApiService : ICommentWebApiService
{
    private readonly HttpClient httpClient;

    public CommentWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CommentModel?> AddAsync(CommentCreateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Comments/";


        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CommentModel>();
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

    public async Task<bool> DeleteAsync(CommentDeleteDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Comments/{model.Id}?taskId={model.TaskId}&createdUserId={model.CreatedUserId}&id={model.Id}";

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

    public async Task<PaginatedResult<CommentModel>> GetAllAsync(CommentFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Comments?pageSize={filter.PageSize}" +
            $"&pageNumber={filter.PageNumber}" +
            $"&taskId={filter.TaskId}";

        if (token != null)
        {

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<CommentModel>>();
                return data ?? new PaginatedResult<CommentModel>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return new PaginatedResult<CommentModel>();
    }

    public async Task<CommentModel?> GetAsync(long id, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Comments/{id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CommentModel>();
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

    public async Task<CommentModel?> UpdateAsync(CommentUpdateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Comments/{model.Id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CommentModel>();
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
