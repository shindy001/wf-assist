using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared;

public static class JsonDefaults
{
    public static JsonSerializerOptions SerializerOptions { get; }

    static JsonDefaults()
    {
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
}