using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Core.Models.Notifications;

[JsonConverter(typeof(JsonStringEnumConverter<ExecutionStatus>))]
internal enum ExecutionStatus
{
    Completed,
    Failed
}

internal sealed record ExecutionEnded : Notification
{
    public required Guid ExecutionId { get; set; }
    public required Guid WorkflowId { get; set; }
    public required ExecutionStatus Status { get; set; }
}

