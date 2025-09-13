namespace WfAssist.AspNetCore.Domain.Workflows.Models;

public class Workflow
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowData Data { get; init; }
}

public sealed record WorkflowData
{
    public required IEnumerable<WorkflowNode> Nodes { get; init; }
    public required IEnumerable<WorkflowEdge> Edges { get; init; }
}

public sealed record WorkflowNode
{
    public required string Id { get; init; }
    public required WorkflowNodeType Type { get; init; }
    public required Position Position { get; init; }
    public required IWorkflowNodeData Data { get; init; }
}

public sealed record WorkflowEdge
{
    public required string Id { get; init; }
    public required Position Position { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}