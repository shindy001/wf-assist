using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

public sealed class Workflow
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
    public required string Source { get; init; }
    public required string Target { get; init; }
}

public sealed record WorkflowNode
{
    public required string Id { get; init; }
    public required Position Position { get; init; }
    public required WorkflowNodeData Data { get; init; }
}

public enum WorkflowNodeDataType
{
    PrintText,
    ExtractProperty,
    Request
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ExtractPropertyNodeData), nameof(WorkflowNodeDataType.ExtractProperty))]
[JsonDerivedType(typeof(PrintTextNodeData), nameof(WorkflowNodeDataType.PrintText))]
[JsonDerivedType(typeof(RequestNodeData), nameof(WorkflowNodeDataType.Request))]
public abstract record WorkflowNodeData;

public sealed record ExtractPropertyNodeData : WorkflowNodeData
{
    public required string Path { get; init; }
    public required string TargetId { get; init; }
}

public sealed record PrintTextNodeData : WorkflowNodeData
{
    public required string Text { get; init; }
    public bool UseConsole { get; init; }
    public string? TargetId { get; init; }
}

public sealed record RequestNodeData : WorkflowNodeData
{
    public required string RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}
