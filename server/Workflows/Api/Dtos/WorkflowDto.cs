using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Api.Dtos;

internal sealed record WorkflowDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowDataDto Data { get; init; }
}

internal sealed record WorkflowDataDto
{
    public IEnumerable<NodeDto> Nodes { get; init; } = [];
    public IEnumerable<EdgeDto> Edges { get; init; } = [];
}

internal sealed record EdgeDto
{
    public required string Id { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RequestNodeDto), nameof(NodeTypeDto.RequestNode))]
[JsonDerivedType(typeof(HeadersNodeDto), nameof(NodeTypeDto.HeadersNode))]
internal abstract record NodeDto
{
    public required string Id { get; init; }

    /// <summary>
    /// Short ID for referencing between nodes. Should be unique only in specific <see cref="WorkflowDto"/> context.
    /// </summary>
    public required string RefId { get; init; }
    public required SizeDto Size { get; init; }
    public required PositionDto Position { get; init; }
}

internal sealed record RequestNodeDto : NodeDto
{
    public required RequestTypeDto RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

internal sealed record HeadersNodeDto : NodeDto
{
    public List<HttpHeaderDto> Headers { get; init; } = [];
}

internal enum NodeTypeDto
{
    RequestNode,
    HeadersNode
}
