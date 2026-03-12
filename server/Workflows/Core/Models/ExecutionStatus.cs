using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ExecutionStatus>))]
public enum ExecutionStatus
{
    Queued,
    Running,
    Completed,
    Failed
}