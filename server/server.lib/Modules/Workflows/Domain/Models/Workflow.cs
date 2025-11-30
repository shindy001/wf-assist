using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

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

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RequestNodeData), nameof(RequestNodeData))]
public abstract record WorkflowNodeData;

public sealed record RequestNodeData : WorkflowNodeData
{
    public required RequestType RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}
