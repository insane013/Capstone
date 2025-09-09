using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoListApp.Services.WebApp.Helpers;
public static class JwtTokenOptions
{
    private static byte[] Key = [];
    private static string Issuer = string.Empty;
    private static string Audience = string.Empty;

    public static void SetOptions(string key, string issuer, string audience)
    {
        Key = Encoding.UTF8.GetBytes(key); ; Issuer = issuer; Audience = audience;
    }

    public static TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };
    }
}
