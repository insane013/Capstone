using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoListApp.Models;
using TodoListApp.Models.User;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;
public class UserWebApiService : IUserWebApiService
{
    public enum UserSearchType
    {
        Id,
        Tag,
        Email,
    }

    private readonly HttpClient httpClient;

    public UserWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ViewUserInfo?> GetUserInfo(string userId, string? token, UserSearchType type = UserSearchType.Id)
    {
        var url = type switch
        {
            UserSearchType.Id => $"{this.httpClient.BaseAddress}Users/Id?userId={userId}",
            UserSearchType.Tag => $"{this.httpClient.BaseAddress}Users/Tag?tag={userId}",
            UserSearchType.Email => $"{this.httpClient.BaseAddress}Users/Email?email={userId}",
            _ => $"{this.httpClient.BaseAddress}Users/Id?userId={userId}"
        };

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ViewUserInfo>();
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

    public async Task<PaginatedResult<ViewUserInfo>> GetUsers(UserFilter filter, string? token)
    {
        var url = $"{this.httpClient.BaseAddress}Users?todoListId={filter.TodoListId}&pageSize={filter.PageSize}&pageNumber={filter.PageNumber}";

        if (filter.Roles.Count > 0)
        {
            foreach (var role in filter.Roles)
            {
                url += $"&roles={role}";
            }
        }

        if (token != null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PaginatedResult<ViewUserInfo>>();
                return data ?? new PaginatedResult<ViewUserInfo>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
        }

        return new PaginatedResult<ViewUserInfo>();
    }
}
