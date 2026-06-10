using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using WfAssist.Shared;
using WfAssist.Shared.CQRS;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Api.Features;

internal static class Delete
{
    internal static void MapDeleteWorkflowEndpoint(this IEndpointRouteBuilder endpoints)
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

internal sealed class DeleteWorkflowCommandHandler(IUnitOfWork uow)
    : ICommandHandler<DeleteWorkflowCommand, Success>
{
    public async Task<Success> Handle(DeleteWorkflowCommand command, CancellationToken cancellationToken = default)
    {
        await uow.GetRepository<Workflow>().Delete(command.Id);
        await uow.SaveChangesAsync(cancellationToken);
        return new Success();
    }
}