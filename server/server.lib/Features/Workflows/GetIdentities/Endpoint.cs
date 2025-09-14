using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WfAssist.AspNetCore.Features.Workflows.GetIdentities;

public static class Endpoint
{
    public static void MapGetIdentitiesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/identities", () =>
            {
                // TODO
                // 1. Create handler and register to services
                // 2. inject handler and from services and call
                return TypedResults.Ok(new GetIdentitiesResponse());
            })
            .Produces<GetIdentitiesResponse>();
    }
}
