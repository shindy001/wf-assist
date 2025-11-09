using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Modules.Workflows.Api.Dtos;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Contracts;

namespace WfAssist.AspNetCore.Modules.Workflows.Features;

public static class GetIdentities
{
    public static void MapGetIdentitiesEndpoint(this IEndpointRouteBuilder endpoints)
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