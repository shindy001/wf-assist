using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Core.Models;
using WfAssist.AspNetCore.Core.Services;

namespace WfAssist.AspNetCore.Api.Workflows.Features;

public static class QueueWorkflowRun
{
    public static void MapQueueRunEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/queueRun", async Task<Results<Ok<RunWorkflowResponse>, NotFound<string>>> (
                Guid id,
                IWorkflowRepository workflowRepository,
                IExecutionRepository executionRepository) =>
            {
                var workflow = await workflowRepository.GetById(id);
                if (workflow is null)
                {
                    return TypedResults.NotFound($"Workflow with ID '{id}' was not found.");
                }

                var execution = ExecutionFactory.CreateQueued(workflow);
                var executionId = await executionRepository.Add(execution);

                return TypedResults.Ok(new RunWorkflowResponse(executionId));
            })
            .Produces<RunWorkflowResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record RunWorkflowResponse(Guid RunId);

}