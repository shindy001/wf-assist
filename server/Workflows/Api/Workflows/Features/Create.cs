using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Workflows.Api.Workflows.Dtos;
using WfAssist.Workflows.Api.Workflows.Mappers;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Workflows.Features;

public static class Create
{
    public static void MapCreateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", async (CreateWorkflowRequest request, IWorkflowRepository workflowRepository) =>
            {
                var newWorkflow = new Workflow
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Data = request.Data.ToDomain()
                };

                await workflowRepository.Create(newWorkflow);

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);
    }

    private sealed record CreateWorkflowRequest(string Name, WorkflowDataDto Data);

}