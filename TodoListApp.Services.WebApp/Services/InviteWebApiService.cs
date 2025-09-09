using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoListApp.Models;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;

public class InviteWebApiService : IInviteWebApiService
{
    private readonly HttpClient httpClient;

    public InviteWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PaginatedResult<InviteModel>> InviteAsync(InviteUsersWebModel model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/";

        var createModel = new InviteCreateDto
        {
            TodoListId = model.TodoListId,
            Message = model.Message,
            Users = model.GetUserList(),
        };

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(createModel);

            var response = await this.httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<InviteModel>>();
                return data ?? new PaginatedResult<InviteModel>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return new PaginatedResult<InviteModel>();
    }

    public async Task<bool> DeleteAsync(InviteDeleteDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/{model.Id}?todoListId={model.TodoListId}&id={model.Id}";

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

    public async Task<bool> ResponseAsync(InviteResponseDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/{model.Id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PostAsync(url, content);

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

    public async Task<PaginatedResult<InviteModel>> GetFromListAsync(InviteFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/List?pageSize={filter.PageSize}" +
            $"&pageNumber={filter.PageNumber}" +
            $"&ForTodoList={filter.ForTodoList}";

        if (!string.IsNullOrEmpty(filter.ForUser))
        {
            url += $"&ForUser={filter.ForUser}";
        }

        if (token != null)
        {

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<InviteModel>>();
                return data ?? new PaginatedResult<InviteModel>();
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

        return new PaginatedResult<InviteModel>();
    }

    public async Task<PaginatedResult<InviteModel>> GetFromUserAsync(InviteFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/CurrentUser?pageSize={filter.PageSize}" +
            $"&pageNumber={filter.PageNumber}";

        if (filter.ForTodoList != 0)
        {
            url += $"&ForTodoList={filter.ForTodoList}";
        }

        if (token != null)
        {

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<InviteModel>>();
                return data ?? new PaginatedResult<InviteModel>();
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

        return new PaginatedResult<InviteModel>();
    }

    public async Task<InviteModel?> GetAsync(long id, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/{id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<InviteModel>();
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

    public async Task<InviteModel?> UpdateAsync(InviteUpdateDto model, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Invites/{model.Id}";

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = JsonContent.Create(model);

            var response = await this.httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<InviteModel>();
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
