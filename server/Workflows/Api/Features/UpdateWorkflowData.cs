using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class UpdateWorkflowData
{
    public static void MapUpdateDataEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/updateData", async (Guid id, UpdateWorkflowDataRequest request, IWorkflowRepository workflowRepository) =>
            {
                await workflowRepository.UpdateData(id, request.Data.ToDomain());

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record UpdateWorkflowDataRequest(WorkflowDataDto Data);

}