using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Domain.Workflows.Contracts;
using WfAssist.AspNetCore.Domain.Workflows.Models;
using WfAssist.AspNetCore.Features.Workflows.Dtos;
using WfAssist.AspNetCore.Features.Workflows.Mappers;

namespace WfAssist.AspNetCore.Features.Workflows;

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