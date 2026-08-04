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
    public IEnumerable<Node> Nodes { get; init; } = [];
    public IEnumerable<Edge> Edges { get; init; } = [];
}

internal sealed record Edge
{
    public required string Id { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RequestNode), nameof(RequestNode))]
[JsonDerivedType(typeof(HeadersNode), nameof(HeadersNode))]
internal abstract record Node
{
    public required string Id { get; init; }
    public required Size Size { get; init; }
    public required Position Position { get; init; }
}

internal sealed record RequestNode : Node
{
    public required RequestType RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

internal sealed record HeadersNode : Node
{
    public List<HttpHeader> Headers { get; init; } = [];
}
