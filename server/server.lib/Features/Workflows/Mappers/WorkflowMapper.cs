using WfAssist.AspNetCore.Domain.Workflows.Models;
using WfAssist.AspNetCore.Features.Workflows.Dtos;

namespace WfAssist.AspNetCore.Features.Workflows.Mappers;

public static class WorkflowMapper
{
    public static WorkflowDto ToDto(this Workflow entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Data = entity.Data.ToDto()
        };

    public static Workflow ToDomain(this WorkflowDto dto)
        => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Data = dto.Data.ToDomain()
        };

    public static WorkflowDataDto ToDto(this WorkflowData entity)
        => new()
        {
            Edges = entity.Edges.Select(ToDto),
            Nodes = entity.Nodes.Select(ToDto)
        };

    public static WorkflowData ToDomain(this WorkflowDataDto dto)
        => new()
        {
            Edges = dto.Edges.Select(ToDomain),
            Nodes = dto.Nodes.Select(ToDomain)
        };

    public static WorkflowEdgeDto ToDto(this WorkflowEdge entity)
        => new()
        {
            Id = entity.Id,
            Position = entity.Position.ToDto(),
            Source = entity.Source,
            Target = entity.Target,
        };

    public static WorkflowEdge ToDomain(this WorkflowEdgeDto dto)
        => new()
        {
            Id = dto.Id,
            Position = dto.Position.ToDomain(),
            Source = dto.Source,
            Target = dto.Target,
        };

    public static WorkflowNodeDto ToDto(this WorkflowNode entity)
    {
        return entity switch
        {
            PrintTextNode printTextNode => new PrintTextNodeDto
            {
                Id = printTextNode.Id,
                Position = printTextNode.Position.ToDto(),
                Text = printTextNode.Text,
                UseConsole = printTextNode.UseConsole,
                TargetId = printTextNode.TargetId
            },
            RequestNode requestNode => new RequestNodeDto
            {
                Id = requestNode.Id,
                Position = requestNode.Position.ToDto(),
                RequestType = requestNode.RequestType,
                Url = requestNode.Url,
                RequestBody = requestNode.RequestBody
            },
            ExtractPropertyNode extractPropertyNode => new ExtractPropertyNodeDto
            {
                Id = extractPropertyNode.Id,
                Position = extractPropertyNode.Position.ToDto(),
                Path = extractPropertyNode.Path,
                TargetId = extractPropertyNode.TargetId
            },
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeDto type {entity.GetType().Name}")
        };
    }

    public static WorkflowNode ToDomain(this WorkflowNodeDto dto)
    {
        return dto switch
        {
            PrintTextNodeDto printTextNodeDto => new PrintTextNode
            {
                Id = printTextNodeDto.Id,
                Position = printTextNodeDto.Position.ToDomain(),
                Text = printTextNodeDto.Text,
                UseConsole = printTextNodeDto.UseConsole,
                TargetId = printTextNodeDto.TargetId
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

    public static PositionDto ToDto(this Position entity) => new(entity.X, entity.Y);
    public static Position ToDomain(this PositionDto dto) => new(dto.X, dto.Y);
}