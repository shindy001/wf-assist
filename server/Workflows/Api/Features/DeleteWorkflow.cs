using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using Shared.CQRS;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class Delete
{
    public static void MapDeleteWorkflowEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/{id:guid}", async (Guid id, ICommandDispatcher commandDispatcher) =>
            {
                await commandDispatcher.Dispatch(new DeleteWorkflowCommand(id));

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);
    }
}

internal record DeleteWorkflowCommand(Guid Id) : ICommand<Success>;

internal sealed class DeleteWorkflowCommandHandler(IWorkflowRepository workflowRepository)
    : ICommandHandler<DeleteWorkflowCommand, Success>
{
    public async Task<Success> Handle(DeleteWorkflowCommand command, CancellationToken cancellationToken = default)
    {
        await workflowRepository.Delete(command.Id);
        return new Success();
    }
}