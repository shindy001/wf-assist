using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Domain.Workflows.Contracts;

namespace WfAssist.AspNetCore.Features.Workflows;

public static class Delete
{
    public static void MapDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/{id:guid}", async (Guid id, IWorkflowRepository workflowRepository) =>
            {
                await workflowRepository.Delete(id);

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);
    }
}