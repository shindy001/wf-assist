namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

public sealed record Execution
{
    public required Guid Id { get; init; }
    public required ExecutionStatus Status { get; init; }
    public required Workflow Snapshot { get; init; }
    public Dictionary<string, ProcessingResult> ProcessingResults { get; init; } = [];
}

public enum ExecutionStatus
{
    Queued,
    Running,
    Completed,
    Failed
}

public static class ExecutionFactory
{
    public static Execution CreateQueued(Workflow workflow)
    {
        return new Execution
        {
            Id = Guid.NewGuid(),
            Status = ExecutionStatus.Queued,
            Snapshot = workflow
        };
    }
}