using System.Text.Json;
using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Infrastructure.Serialization;

public static class DbJsonSerializerOptions
{
    public static JsonSerializerOptions Value { get; }

    static DbJsonSerializerOptions()
    {
        Value = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        Value.Converters.Add(new JsonStringEnumConverter());
    }


}