using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Modules.Workflows.Api.Dtos;

[JsonConverter(typeof(JsonStringEnumConverter<RequestTypeDto>))]
public enum RequestTypeDto
{
    Get,
    Post,
    Put,
    Patch,
    Delete
}