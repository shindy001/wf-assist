using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class GetWorkflowById
{
    public static void MapGetWorkflowByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/{id:guid}", async (Guid id, IWorkflowRepository workflowRepository) =>
            {
                var workflow = await workflowRepository.GetById(id);
                if (workflow is null)
                {
                    return Results.NotFound($"Workflow with id '{id}' not found");
                }

                var response = new GetWorkflowByIdResponse(workflow.ToDto());
                return TypedResults.Ok(response);
            })
            .Produces<GetWorkflowByIdResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record GetWorkflowByIdResponse(WorkflowDto Item);

}