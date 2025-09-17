using WfAssist.AspNetCore.Domain.Workflows.Models;
using WfAssist.AspNetCore.Features.Workflows.Dtos;

namespace WfAssist.AspNetCore.Features.Workflows.Mappers;

public static class WorkflowMapper
{
    public static WorkflowData ToDomain(this WorkflowDataDto dto)
        => new()
        {
            Edges = dto.Edges.Select(ToDomain),
            Nodes = dto.Nodes.Select(ToDomain)
        };

    public static WorkflowEdge ToDomain(this WorkflowEdgeDto dto)
        => new()
        {
            Id = dto.Id,
            Position = dto.Position.ToDomain(),
            Source = dto.Source,
            Target = dto.Target,
        };

    public static WorkflowNode ToDomain(this WorkflowNodeDto dto)
    {
        return dto switch
        {
            PrintTextNodeDto printTextDto => new PrintTextNode
            {
                Id = printTextDto.Id,
                Position = printTextDto.Position.ToDomain(),
                Text = printTextDto.Text,
                UseConsole = printTextDto.UseConsole,
                TargetId = printTextDto.TargetId
            },
            RequestNodeDto requestNodeDto => new RequestNode
            {
                Id = requestNodeDto.Id,
                Position = requestNodeDto.Position.ToDomain(),
                RequestType = requestNodeDto.RequestType,
                Url = requestNodeDto.Url,
                RequestBody = requestNodeDto.RequestBody
            },
            ExtractPropertyNodeDto extractPropertyNodeDto => new ExtractPropertyNode
            {
                Id = extractPropertyNodeDto.Id,
                Position = extractPropertyNodeDto.Position.ToDomain(),
                Path = extractPropertyNodeDto.Path,
                TargetId = extractPropertyNodeDto.TargetId
            },
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeDto type {dto.GetType().Name}")
        };
    }

    public static Position ToDomain(this PositionDto dto) => new(dto.X, dto.Y);
}