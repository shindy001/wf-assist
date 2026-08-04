using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Api.Mappers;

internal static class WorkflowMapper
{
    internal static WorkflowDto ToDto(this Workflow entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Data = entity.Data.ToDto()
        };

    internal static Workflow ToDomain(this WorkflowDto dto)
        => new(dto.Id, dto.Name, dto.Data.ToDomain());

    internal static WorkflowData ToDomain(this WorkflowDataDto dto)
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

    private static EdgeDto ToDto(this Edge entity)
        => new()
        {
            Id = entity.Id,
            Source = entity.Source,
            Target = entity.Target,
        };

    private static Edge ToDomain(this EdgeDto dto)
        => new()
        {
            Id = dto.Id,
            Source = dto.Source,
            Target = dto.Target,
        };

    private static NodeDto ToDto(this Node entity)
    {
        return entity switch
        {
            RequestNode requestNode => new RequestNodeDto
            {
                Id = requestNode.Id,
                Size = new SizeDto(requestNode.Size.Width, requestNode.Size.Height),
                Position = new PositionDto(requestNode.Position.X, requestNode.Position.Y),
                RequestType = requestNode.RequestType.ToDto(),
                Url = requestNode.Url,
                RequestBody = requestNode.RequestBody
            },
            HeadersNode headersNode => new HeadersNodeDto
            {
                Id = headersNode.Id,
                Size = new SizeDto(headersNode.Size.Width, headersNode.Size.Height),
                Position = new PositionDto(headersNode.Position.X, headersNode.Position.Y),
                Headers = headersNode.Headers.Select(x => new HttpHeaderDto(x.Name, x.Value)).ToList()
            },
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    private static Node ToDomain(this NodeDto dto)
    {
        return dto switch
        {
            RequestNodeDto requestNodeDto => new RequestNode
            {
                Id = requestNodeDto.Id,
                Size = new Size(requestNodeDto.Size.Width, requestNodeDto.Size.Height),
                Position = new Position(requestNodeDto.Position.X, requestNodeDto.Position.Y),
                RequestType = requestNodeDto.RequestType.ToDomain(),
                Url = requestNodeDto.Url,
                RequestBody = requestNodeDto.RequestBody
            },
            HeadersNodeDto headersNodeDto => new HeadersNode
            {
                Id = headersNodeDto.Id,
                Size = new Size(headersNodeDto.Size.Width, headersNodeDto.Size.Height),
                Position = new Position(headersNodeDto.Position.X, headersNodeDto.Position.Y),
                Headers = headersNodeDto.Headers.Select(x => new HttpHeader(x.Name, x.Value)).ToList()
            },
            _ => throw new ArgumentOutOfRangeException(nameof(dto))
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
