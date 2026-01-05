namespace WfAssist.AspNetCore.Core.Models.Notifications;

internal sealed record ExecutionEnded : Notification
{
    public required Guid ExecutionId { get; set; }
    public required Guid WorkflowId { get; set; }
    public required ExecutionStatus Status { get; set; }
}

