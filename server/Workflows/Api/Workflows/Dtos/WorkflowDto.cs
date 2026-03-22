using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Api.Workflows.Dtos;

internal class WorkflowDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowDataDto Data { get; init; }
}

internal sealed record WorkflowDataDto
{
    public IEnumerable<WorkflowNodeDto> Nodes { get; init; } = [];
    public IEnumerable<WorkflowEdgeDto> Edges { get; init; } = [];
}

internal sealed record WorkflowEdgeDto
{
    public required string Id { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}

internal sealed record WorkflowNodeDto
{
    public required string Id { get; init; }
    public required PositionDto Position { get; init; }
    public required WorkflowNodeDataDto Data { get; init; }
}

internal enum WorkflowNodeDataTypeDto
{
    Request,
    Headers
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RequestNodeDataDto), nameof(WorkflowNodeDataTypeDto.Request))]
[JsonDerivedType(typeof(HeadersNodeDataDto), nameof(WorkflowNodeDataTypeDto.Headers))]
internal abstract record WorkflowNodeDataDto;

internal sealed record RequestNodeDataDto : WorkflowNodeDataDto
{
    public required RequestTypeDto RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

internal sealed record HeadersNodeDataDto : WorkflowNodeDataDto
{
    public List<HttpHeaderDto> Headers { get; init; } = [];
}
