using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using Shared.CQRS;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class Create
{
    public static void MapCreateWorkflowEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", async (CreateWorkflowRequest request, ICommandDispatcher commandDispatcher) =>
            {
                await commandDispatcher.Dispatch(new CreateWorkflowCommand(request.Name, request.Data.ToDomain()));

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);
    }

    private sealed record CreateWorkflowRequest(string Name, WorkflowDataDto Data);

}

internal record CreateWorkflowCommand(string Name, WorkflowData Data) : ICommand<Success>;

internal sealed class CreateWorkflowCommandHandler(IWorkflowRepository workflowRepository)
    : ICommandHandler<CreateWorkflowCommand, Success>
{
    public async Task<Success> Handle(CreateWorkflowCommand command, CancellationToken cancellationToken = default)
    {
        var newWorkflow = new Workflow {Id = Guid.NewGuid(), Name = command.Name, Data = command.Data};
        await workflowRepository.Create(newWorkflow);
        return new Success();
    }
}