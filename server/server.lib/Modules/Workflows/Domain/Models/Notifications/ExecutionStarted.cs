namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models.Notifications;

internal sealed record ExecutionStarted : Notification
{
    public required Guid ExecutionId { get; set; }
    public required Guid WorkflowId { get; set; }
}