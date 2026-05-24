namespace WfAssist.Executions.Core.Models;

internal sealed record ExecutionIdentity
{
    public required Guid Id { get; init; }
    public required ExecutionStatus Status { get; init; }
}