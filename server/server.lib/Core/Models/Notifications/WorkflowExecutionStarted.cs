namespace WfAssist.AspNetCore.Core.Models.Notifications;

internal sealed record WorkflowExecutionStarted : Notification
{
    public required Guid ExecutionId { get; init; }
    public required Guid WorkflowId { get; init; }
    public required string WorkflowName { get; init; }
}