using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class GetIdentities
{
    public static void MapGetWorkflowIdentitiesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/identities", async (IWorkflowRepository workflowRepository) =>
            {
                var identities = await workflowRepository.GetIdentities();
                var response = new GetIdentitiesResponse
                {
                    Identities = identities.Select(x => new WorkFlowIdentityDto(x.Id, x.Name))
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