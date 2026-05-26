using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Core.Models;

internal sealed class Workflow
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public WorkflowData Data { get; private set; }

    public Workflow(Guid id, string name, WorkflowData data)
    {
        Id = id;
        Name = name;
        Data = data;
    }

    public void ChangeName(string newName) => Name = newName;
    public void ChangeData(WorkflowData data) => Data = data;
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
