using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ExecutionStatus>))]
internal enum ExecutionStatus
{
    Queued,
    Running,
    Completed,
    Failed
}