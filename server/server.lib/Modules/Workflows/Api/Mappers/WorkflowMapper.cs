using WfAssist.AspNetCore.Modules.Workflows.Api.Dtos;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Api.Mappers;

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

    public static WorkflowData ToDomain(this WorkflowDataDto dto)
        => new()
        {
            Edges = dto.Edges.Select(ToDomain),
            Nodes = dto.Nodes.Select(ToDomain)
        };

    private static WorkflowDataDto ToDto(this WorkflowData entity)
        => new()
        {
            Edges = entity.Edges.Select(ToDto),
            Nodes = entity.Nodes.Select(ToDto)
        };

    private static WorkflowEdgeDto ToDto(this WorkflowEdge entity)
        => new()
        {
            Id = entity.Id,
            Source = entity.Source,
            Target = entity.Target,
        };

    private static WorkflowEdge ToDomain(this WorkflowEdgeDto dto)
        => new()
        {
            Id = dto.Id,
            Source = dto.Source,
            Target = dto.Target,
        };

    private static WorkflowNodeDto ToDto(this WorkflowNode entity)
    {
        return new WorkflowNodeDto
        {
            Id = entity.Id,
            Position = new PositionDto(entity.Position.X, entity.Position.Y),
            Data = entity.Data.ToDto()
        };
    }

    private static WorkflowNode ToDomain(this WorkflowNodeDto dto)
    {
        return new WorkflowNode
        {
            Id = dto.Id,
            Position = new Position(dto.Position.X, dto.Position.Y),
            Data = dto.Data.ToDomain()
        };
    }

    private static WorkflowNodeDataDto ToDto(this WorkflowNodeData workflowNodeData)
    {
        return workflowNodeData switch
        {
            RequestNodeData data => new RequestNodeDataDto
            {
                RequestType = data.RequestType.ToDto(),
                Url = data.Url,
                RequestBody = data.RequestBody
            },
            HeadersNodeData data => new HeadersNodeDataDto
            {
                Headers = data.Headers.Select(x => new HttpHeaderDto(x.Name, x.Value)).ToList()
            },
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeData type {workflowNodeData.GetType().Name}")
        };
    }

    private static WorkflowNodeData ToDomain(this WorkflowNodeDataDto dto)
    {
        return dto switch
        {
            RequestNodeDataDto data => new RequestNodeData
            {
                RequestType = data.RequestType.ToDomain(),
                Url = data.Url,
                RequestBody = data.RequestBody
            },
            HeadersNodeDataDto data => new HeadersNodeData
            {
                Headers = data.Headers.Select(x => new HttpHeader(x.Name, x.Value)).ToList()
            },
            _ => throw new InvalidOperationException($"Unknown WorkflowNodeDataDto type {dto.GetType().Name}")
        };
    }

    private static RequestType ToDomain(this RequestTypeDto dto)
    {
        return dto switch
        {
            RequestTypeDto.Get => RequestType.Get,
            RequestTypeDto.Post => RequestType.Post,
            RequestTypeDto.Put => RequestType.Put,
            RequestTypeDto.Patch => RequestType.Patch,
            RequestTypeDto.Delete => RequestType.Delete,
            _ => throw new InvalidOperationException($"Unknown RequestType type {dto}")
        };
    }

    private static RequestTypeDto ToDto(this RequestType requestType)
    {
        return requestType switch
        {
            RequestType.Get => RequestTypeDto.Get,
            RequestType.Post => RequestTypeDto.Post,
            RequestType.Put => RequestTypeDto.Put,
            RequestType.Patch => RequestTypeDto.Patch,
            RequestType.Delete => RequestTypeDto.Delete,
            _ => throw new InvalidOperationException($"Unknown RequestType type {requestType}")
        };
    }
}
