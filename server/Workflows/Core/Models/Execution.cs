namespace WfAssist.Workflows.Core.Models;

public sealed record Execution
{
    public required Guid Id { get; init; }
    public required ExecutionStatus Status { get; init; }
    public required WorkflowSnapshot Snapshot { get; init; }
    public Dictionary<string, ProcessingResult> ProcessingResults { get; init; } = [];
}

public static class ExecutionFactory
{
    public static Execution CreateQueued(Workflow workflow)
    {
        return new Execution
        {
            Id = Guid.NewGuid(),
            Status = ExecutionStatus.Queued,
            Snapshot = new WorkflowSnapshot
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Data = workflow.Data
            }
        };
    }
}