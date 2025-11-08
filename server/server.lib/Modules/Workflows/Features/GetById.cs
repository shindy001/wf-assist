using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Domain.Workflows.Contracts;
using WfAssist.AspNetCore.Modules.Workflows.Features.Dtos;
using WfAssist.AspNetCore.Modules.Workflows.Features.Mappers;

namespace WfAssist.AspNetCore.Modules.Workflows.Features;

public static class GetById
{
    public static void MapGetByIdEndpoint(this IEndpointRouteBuilder endpoints)
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