namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

public sealed record WorkflowRunIdentity
{
    public required Guid Id { get; init; }
    public required WorkflowRunStatus Status { get; init; }
}