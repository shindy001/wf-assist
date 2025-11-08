using WfAssist.AspNetCore.Domain.Workflows.Models;
using WfAssist.AspNetCore.Modules.Workflows.Features.Dtos;

namespace WfAssist.AspNetCore.Modules.Workflows.Features.Mappers;

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
            Source = entity.Source,
            Target = entity.Target,
        };

    public static WorkflowEdge ToDomain(this WorkflowEdgeDto dto)
        => new()
        {
            Id = dto.Id,
            Source = dto.Source,
            Target = dto.Target,
        };

    public static WorkflowNodeDto ToDto(this WorkflowNode entity)
    {
        return new WorkflowNodeDto
        {
            Id = entity.Id,
            Position = entity.Position.ToDto(),
            Data = entity.Data.ToDto()
        };
    }

    public static WorkflowNode ToDomain(this WorkflowNodeDto dto)
    {
        return new WorkflowNode
        {
            Id = dto.Id,
            Position = dto.Position.ToDomain(),
            Data = dto.Data.ToDomain()
        };
    }

    public static WorkflowNodeDataDto ToDto(this WorkflowNodeData workflowNodeData)
    {
        return workflowNodeData switch
        {
            PrintTextNodeData data => new PrintTextNodeDataDto
            {
                Text = data.Text,
                UseConsole = data.UseConsole,
                TargetId = data.TargetId
            },
            ExtractPropertyNodeData data => new ExtractPropertyNodeDataDto
            {
                Path = data.Path,
                TargetId = data.TargetId
            },
            RequestNodeData data => new RequestNodeDataDto
            {
                RequestType = data.RequestType,
                Url = data.Url,
                RequestBody = data.RequestBody
            },
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeData type {workflowNodeData.GetType().Name}")
        };
    }

    public static WorkflowNodeData ToDomain(this WorkflowNodeDataDto dto)
    {
        return dto switch
        {
            PrintTextNodeDataDto data => new PrintTextNodeData
            {
                Text = data.Text,
                UseConsole = data.UseConsole,
                TargetId = data.TargetId
            },
            ExtractPropertyNodeDataDto data => new ExtractPropertyNodeData
            {
                Path = data.Path,
                TargetId = data.TargetId
            },
            RequestNodeDataDto data => new RequestNodeData
            {
                RequestType = data.RequestType,
                Url = data.Url,
                RequestBody = data.RequestBody
            },
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeDataDto type {dto.GetType().Name}")
        };
    }

    public static WorkflowNodeDataTypeDto ToDto(this WorkflowNodeDataType nodeDataType)
    {
        return nodeDataType switch
        {
            WorkflowNodeDataType.PrintText => WorkflowNodeDataTypeDto.PrintText,
            WorkflowNodeDataType.ExtractProperty => WorkflowNodeDataTypeDto.ExtractProperty,
            WorkflowNodeDataType.Request => WorkflowNodeDataTypeDto.Request,
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeType type {nodeDataType}")
        };
    }

    public static WorkflowNodeDataType ToDomain(this WorkflowNodeDataTypeDto nodeDataTypeDto)
    {
        return nodeDataTypeDto switch
        {
            WorkflowNodeDataTypeDto.PrintText => WorkflowNodeDataType.PrintText,
            WorkflowNodeDataTypeDto.ExtractProperty => WorkflowNodeDataType.ExtractProperty,
            WorkflowNodeDataTypeDto.Request => WorkflowNodeDataType.Request,
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeTypeDto type {nodeDataTypeDto}")
        };
    }

    public static PositionDto ToDto(this Position entity) => new(entity.X, entity.Y);
    public static Position ToDomain(this PositionDto dto) => new(dto.X, dto.Y);
}