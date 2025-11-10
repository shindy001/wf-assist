namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

public sealed record WorkflowRun
{
    public required Guid Id { get; init; }
    public required WorkflowRunStatus Status { get; init; }
    public required WorkflowSnapshot Snapshot { get; init; }
}

public enum WorkflowRunStatus
{
    Queued,
    Running,
    Completed,
    Failed
}

public static class WorkflowRunFactory
{
    public static WorkflowRun CreateQueued(Workflow workflow)
    {
        return new WorkflowRun
        {
            Id = Guid.NewGuid(),
            Status = WorkflowRunStatus.Queued,
            Snapshot = new WorkflowSnapshot
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Data = workflow.Data
            }
        };
    }
}