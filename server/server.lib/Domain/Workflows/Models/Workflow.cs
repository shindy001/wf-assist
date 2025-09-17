namespace WfAssist.AspNetCore.Domain.Workflows.Models;

public class Workflow
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowData Data { get; init; }
}

public sealed record WorkflowData
{
    public IEnumerable<WorkflowNode> Nodes { get; init; } = [];
    public IEnumerable<WorkflowEdge> Edges { get; init; } = [];
}

public sealed record WorkflowEdge
{
    public required string Id { get; init; }
    public required Position Position { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}


public abstract record WorkflowNode(WorkflowNodeType Type)
{
    public WorkflowNodeType Type { get; } = Type;
    public required string Id { get; init; }
    public required Position Position { get; init; }
}

public sealed record PrintTextNode() : WorkflowNode(WorkflowNodeType.PrintText)
{
    public required string Text { get; init; }
    public bool UseConsole { get; init; }
    public string? TargetId { get; init; }
}

public sealed record RequestNode() : WorkflowNode(WorkflowNodeType.Request)
{
    public required string RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

public sealed record ExtractPropertyNode() : WorkflowNode(WorkflowNodeType.ExtractProperty)
{
    public required string Path { get; init; }
    public required string TargetId { get; init; }
}
