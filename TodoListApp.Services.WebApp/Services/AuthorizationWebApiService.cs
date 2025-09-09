using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.Helpers;
using TodoListApp.Models.User.Authorization;
using TodoListApp.Models.WebApp;
using TodoListApp.Services.WebApp.Helpers;
using TodoListApp.Services.WebApp.Interfaces;

namespace TodoListApp.Services.WebApp.Services;
public class AuthorizationWebApiService : IAuthorizationWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<AuthorizationWebApiService> logger;

    public AuthorizationWebApiService(HttpClient httpClient, ILogger<AuthorizationWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<string> LoginToApi(LoginUserModel model)
    {
        LoggingDelegates.LogInfo(this.logger, "Login request to API.", null);

        var response = await this.httpClient.PostAsJsonAsync($"{this.httpClient.BaseAddress}authorization/Login", model);


        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

            return content!.Token;
        }
        else
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
            catch (JsonException)
            {
                var error = "Cannot reach an API.";
                throw new ApplicationException(error);
            }
        }
    }

    public async Task<bool> Register(RegisterUserModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{this.httpClient.BaseAddress}authorization/register", model);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorWebModel>();
                throw new ApplicationException(error?.Message ?? "Unexpected error.");
            }
            catch (JsonException)
            {
                var error = "Cannot reach an API.";
                throw new ApplicationException(error);
            }
        }
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = JwtTokenOptions.GetValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is JwtSecurityToken jwt &&
                jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch (Exception ex) when (ex is not ApplicationException) // any kind of exception. SA1031 -_-.
        {
            return null;
        }

        return null;
    }
}
