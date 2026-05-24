namespace WfAssist.Workflows.Core.Models.Notifications;

internal sealed record WorkflowExecutionEnded : Notification
{
    public required Guid ExecutionId { get; init; }
    public required Guid WorkflowId { get; init; }
    public required string WorkflowName { get; init; }
    public required string Status { get; init; }
}

