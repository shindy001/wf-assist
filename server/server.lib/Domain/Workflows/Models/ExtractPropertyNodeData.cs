namespace WfAssist.AspNetCore.Domain.Workflows.Models;

public sealed record ExtractPropertyNodeData() : WorkflowNodeDataBase(WorkflowNodeType.ExtractProperty)
{
    public required string Path { get; init; }
    public required string TargetId { get; init; }
}