using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Api.Workflows.Dtos;

[JsonConverter(typeof(JsonStringEnumConverter<RequestTypeDto>))]
internal enum RequestTypeDto
{
    Get,
    Post,
    Put,
    Patch,
    Delete
}