namespace WfAssist.AspNetCore.Core.Models;

public sealed record ExecutionIdentity
{
    public required Guid Id { get; init; }
    public required ExecutionStatus Status { get; init; }
}