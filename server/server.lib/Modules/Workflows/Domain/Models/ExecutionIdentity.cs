namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

public sealed record ExecutionIdentity
{
    public required Guid Id { get; init; }
    public required ExecutionStatus Status { get; init; }
}