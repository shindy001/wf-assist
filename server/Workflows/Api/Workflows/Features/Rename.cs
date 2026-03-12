using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Workflows.Features;

public static class Rename
{
    public static void MapRenameEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/rename", async (Guid id, RenameWorkflowRequest request, IWorkflowRepository workflowRepository) =>
            {
                if (!await workflowRepository.Exists(id))
                {
                    return Results.NotFound($"Workflow with id '{id}' not found");
                }

                await workflowRepository.Rename(id, request.NewName);

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record RenameWorkflowRequest(string NewName);

}