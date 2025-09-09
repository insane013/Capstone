using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoListApp.WebApi.Helpers;

/// <summary>
/// Authorization options.
/// </summary>
internal static class AuthOptions
{
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string? key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!));
    }
}
