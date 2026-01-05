using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ExecutionStatus>))]
public enum ExecutionStatus
{
    Queued,
    Running,
    Completed,
    Failed
}