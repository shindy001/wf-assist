using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Contracts;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Features;

public static class QueueWorkflowRun
{
    public static void MapQueueRunEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/queueRun", async Task<Results<Ok<RunWorkflowResponse>, NotFound<string>>> (
                Guid id,
                IWorkflowRepository workflowRepository,
                IWorkflowProcessingRepository workflowProcessingRepository) =>
            {
                var workflow = await workflowRepository.GetById(id);
                if (workflow is null)
                {
                    return TypedResults.NotFound($"Workflow with ID '{id}' was not found.");
                }

                var queuedRun = WorkflowRunFactory.CreateQueued(workflow);
                var runId = await workflowProcessingRepository.QueueRun(queuedRun);

                return TypedResults.Ok(new RunWorkflowResponse(runId));
            })
            .Produces<RunWorkflowResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record RunWorkflowResponse(Guid RunId);

}