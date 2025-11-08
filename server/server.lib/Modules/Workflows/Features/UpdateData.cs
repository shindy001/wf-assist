using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Domain.Workflows.Contracts;
using WfAssist.AspNetCore.Modules.Workflows.Features.Dtos;
using WfAssist.AspNetCore.Modules.Workflows.Features.Mappers;

namespace WfAssist.AspNetCore.Modules.Workflows.Features;

public static class UpdateData
{
    public static void MapUpdateDataEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/updateData", async (Guid id, UpdateWorkflowDataRequest request, IWorkflowRepository workflowRepository) =>
            {
                if (!await workflowRepository.Exists(id))
                {
                    return Results.NotFound($"Workflow with id '{id}' not found");
                }

                await workflowRepository.UpdateData(id, request.Data.ToDomain());

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record UpdateWorkflowDataRequest(WorkflowDataDto Data);

}