using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

namespace WfAssist.AspNetCore.Modules.Workflows.Features;

public static class Delete
{
    public static void MapDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/{id:guid}", async (Guid id, WorkflowRepository workflowRepository) =>
            {
                await workflowRepository.Delete(id);

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);
    }
}