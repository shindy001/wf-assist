using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Shared;
using WfAssist.Shared.CQRS;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Api.Features;

internal static class Create
{
    internal static void MapCreateWorkflowEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", async (CreateWorkflowRequest request, ICommandDispatcher commandDispatcher) =>
            {
                var newId = await commandDispatcher.Dispatch(new CreateWorkflowCommand(request.Name, request.Data.ToDomain()));

                return TypedResults.Ok(new CreateWorkflowResponse(newId));
            })
            .Produces<CreateWorkflowResponse>();
    }

    private sealed record CreateWorkflowRequest(string Name, WorkflowDataDto Data);
    private sealed record CreateWorkflowResponse(Guid Id);
}

internal record CreateWorkflowCommand(string Name, WorkflowData Data) : ICommand<Guid>;

internal sealed class CreateWorkflowCommandHandler(IUnitOfWork uow)
    : ICommandHandler<CreateWorkflowCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkflowCommand command, CancellationToken cancellationToken = default)
    {
        var newId = Guid.NewGuid();
        uow.GetRepository<Workflow>().Add(new Workflow(newId, command.Name, command.Data));
        await uow.SaveChangesAsync(cancellationToken);

        return newId;
    }
}