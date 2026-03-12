using System.Text.Json.Serialization;

namespace WfAssist.Workflows.Api.Workflows.Dtos;

public class WorkflowDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowDataDto Data { get; init; }
}

public sealed record WorkflowDataDto
{
    public IEnumerable<WorkflowNodeDto> Nodes { get; init; } = [];
    public IEnumerable<WorkflowEdgeDto> Edges { get; init; } = [];
}

public sealed record WorkflowEdgeDto
{
    public required string Id { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}

public sealed record WorkflowNodeDto
{
    public required string Id { get; init; }
    public required PositionDto Position { get; init; }
    public required WorkflowNodeDataDto Data { get; init; }
}

public enum WorkflowNodeDataTypeDto
{
    Request,
    Headers
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RequestNodeDataDto), nameof(WorkflowNodeDataTypeDto.Request))]
[JsonDerivedType(typeof(HeadersNodeDataDto), nameof(WorkflowNodeDataTypeDto.Headers))]
public abstract record WorkflowNodeDataDto;

public sealed record RequestNodeDataDto : WorkflowNodeDataDto
{
    public required RequestTypeDto RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

public sealed record HeadersNodeDataDto : WorkflowNodeDataDto
{
    public List<HttpHeaderDto> Headers { get; init; } = [];
}
