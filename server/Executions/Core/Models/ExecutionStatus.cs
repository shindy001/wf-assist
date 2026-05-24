using System.Text.Json.Serialization;

namespace WfAssist.Executions.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ExecutionStatus>))]
internal enum ExecutionStatus
{
    Queued,
    Running,
    Completed,
    Failed
}