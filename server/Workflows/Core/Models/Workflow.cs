using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Core.Models;

internal sealed class Workflow
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowData Data { get; init; }
}

internal sealed record WorkflowData
{
    public IEnumerable<WorkflowNode> Nodes { get; init; } = [];
    public IEnumerable<WorkflowEdge> Edges { get; init; } = [];
}

internal sealed record WorkflowEdge
{
    public required string Id { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}

internal sealed record WorkflowNode
{
    public required string Id { get; init; }
    public required Position Position { get; init; }
    public required WorkflowNodeData Data { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RequestNodeData), nameof(RequestNodeData))]
[JsonDerivedType(typeof(HeadersNodeData), nameof(HeadersNodeData))]
internal abstract record WorkflowNodeData;

internal sealed record RequestNodeData : WorkflowNodeData
{
    public required RequestType RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

internal sealed record HeadersNodeData : WorkflowNodeData
{
    public List<HttpHeader> Headers { get; init; } = [];
}
