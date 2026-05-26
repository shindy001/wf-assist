using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using WfAssist.Shared;
using WfAssist.Shared.CQRS;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Models;

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

internal sealed class CreateWorkflowCommandHandler(IUnitOfWork uow)
    : ICommandHandler<CreateWorkflowCommand, Success>
{
    public async Task<Success> Handle(CreateWorkflowCommand command, CancellationToken cancellationToken = default)
    {
        uow.GetRepository<Workflow>().Add(new Workflow(Guid.NewGuid(), command.Name, command.Data));
        await uow.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}