using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Features.Workflows.Dtos;

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
    public required PositionDto Position { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PrintTextNodeDto), nameof(WorkflowNodeTypeDto.PrintText))]
[JsonDerivedType(typeof(RequestNodeDto), nameof(WorkflowNodeTypeDto.Request))]
[JsonDerivedType(typeof(ExtractPropertyNodeDto), nameof(WorkflowNodeTypeDto.ExtractProperty))]
public abstract record WorkflowNodeDto
{
    public required string Id { get; init; }
    public required PositionDto Position { get; init; }
}

public sealed record PrintTextNodeDto : WorkflowNodeDto
{
    public required string Text { get; init; }
    public bool UseConsole { get; init; }
    public string? TargetId { get; init; }
}

public sealed record RequestNodeDto : WorkflowNodeDto
{
    public required string RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

public sealed record ExtractPropertyNodeDto : WorkflowNodeDto
{
    public required string Path { get; init; }
    public required string TargetId { get; init; }
}
