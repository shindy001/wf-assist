using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Domain.Workflows.Contracts;
using WfAssist.AspNetCore.Domain.Workflows.Models;

namespace WfAssist.AspNetCore.Features.Workflows;

public static class Create
{
    public static void MapCreateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", async (CreateWorkflowRequest request, IWorkflowRepository workflowRepository) =>
            {
                await workflowRepository.Create(new Workflow
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Data = new WorkflowData() // TODO - Define WorkflowData model for CreateWorkflowRequest and map to internal WorkflowData
                });

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);
    }

    private sealed record CreateWorkflowRequest(string Name);

}