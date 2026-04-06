using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using OneOf;
using OneOf.Types;
using Shared;
using Shared.CQRS;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;
using NotFound = OneOf.Types.NotFound;

namespace WfAssist.Workflows.Api.Features;

public static class QueueWorkflowExecution
{
    public static void MapQueueWorkflowExecutionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/queueExecution", async Task<Results<Ok<RunWorkflowResponse>, NotFound<string>>> (
                Guid id,
                ICommandDispatcher commandDispatcher) =>
            {
                var result = await commandDispatcher.Dispatch(new QueueWorkflowExecutionCommand(id));

                return result.Match<Results<Ok<RunWorkflowResponse>, NotFound<string>>>(
                    executionId => TypedResults.Ok(new RunWorkflowResponse(executionId.Value)),
                    notFoundError => TypedResults.NotFound(notFoundError.Message));
            })
            .Produces<RunWorkflowResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record RunWorkflowResponse(Guid RunId);

}

internal record QueueWorkflowExecutionCommand(Guid Id) : ICommand<OneOf<Success<Guid>, NotFoundError>>;

internal sealed class QueueWorkflowExecutionCommandHandler(IWorkflowRepository workflowRepository, IExecutionRepository executionRepository)
    : ICommandHandler<QueueWorkflowExecutionCommand, OneOf<Success<Guid>, NotFoundError>>
{
    public async Task<OneOf<Success<Guid>, NotFoundError>> Handle(QueueWorkflowExecutionCommand command,
        CancellationToken cancellationToken = default)
    {
        var workflow = await workflowRepository.GetById(command.Id);
        if (workflow is null)
        {
            return new NotFoundError($"Workflow with ID '{command.Id}' was not found.");
        }

        var execution = ExecutionFactory.CreateQueued(workflow);
        var executionId = await executionRepository.Add(execution);
        return new Success<Guid>(executionId);
    }
}