using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoListApp.Services.WebApp.Helpers;

public static class JsonSerializationHelper
{
    public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
    };
}
