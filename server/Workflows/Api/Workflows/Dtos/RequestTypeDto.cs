using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Api.Workflows.Dtos;

[JsonConverter(typeof(JsonStringEnumConverter<RequestTypeDto>))]
public enum RequestTypeDto
{
    Get,
    Post,
    Put,
    Patch,
    Delete
}