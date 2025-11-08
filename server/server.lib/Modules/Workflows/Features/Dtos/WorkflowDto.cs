using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Modules.Workflows.Features.Dtos;

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

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ExtractPropertyNodeDataDto), nameof(WorkflowNodeDataTypeDto.ExtractProperty))]
[JsonDerivedType(typeof(PrintTextNodeDataDto), nameof(WorkflowNodeDataTypeDto.PrintText))]
[JsonDerivedType(typeof(RequestNodeDataDto), nameof(WorkflowNodeDataTypeDto.Request))]
public abstract record WorkflowNodeDataDto;

public sealed record ExtractPropertyNodeDataDto : WorkflowNodeDataDto
{
    public required string Path { get; init; }
    public required string TargetId { get; init; }
}

public sealed record PrintTextNodeDataDto : WorkflowNodeDataDto
{
    public required string Text { get; init; }
    public bool UseConsole { get; init; }
    public string? TargetId { get; init; }
}

public sealed record RequestNodeDataDto : WorkflowNodeDataDto
{
    public required string RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}
