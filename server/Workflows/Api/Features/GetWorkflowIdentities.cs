using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using WfAssist.Shared.CQRS;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Infrastructure;

namespace WfAssist.Workflows.Api.Features;

internal static class GetIdentities
{
    internal static void MapGetWorkflowIdentitiesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/identities", async (IQueryDispatcher queryDispatcher) =>
            {
                var identities = await queryDispatcher.Dispatch(new GetWorkflowIdentitiesQuery());
                var response = new GetIdentitiesResponse
                {
                    Identities = identities.Select(x => x.ToDto())
                };

                return TypedResults.Ok(response);
            })
            .Produces<GetIdentitiesResponse>();
    }

    private sealed record GetIdentitiesResponse
    {
        public required IEnumerable<WorkFlowIdentityDto> Identities { get; init; } = [];
    }
}

internal record GetWorkflowIdentitiesQuery : IQuery<IEnumerable<WorkflowIdentity>>;

internal sealed class GetWorkflowIdentitiesQueryHandler(WorkflowsDbContext dbContext)
    : IQueryHandler<GetWorkflowIdentitiesQuery, IEnumerable<WorkflowIdentity>>
{
    public async Task<IEnumerable<WorkflowIdentity>> Handle(GetWorkflowIdentitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Workflows.Select(x => new WorkflowIdentity(x.Id, x.Name)).ToListAsync(cancellationToken);
    }
}