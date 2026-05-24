using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using OneOf;
using OneOf.Types;
using WfAssist.Shared;
using WfAssist.Shared.CQRS;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class RenameWorkflow
{
    public static void MapRenameWorkflowEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/rename",
                async (Guid id, RenameWorkflowRequest request, ICommandDispatcher commandDispatcher) =>
                {
                    var result = await commandDispatcher.Dispatch(new RenameWorkflowCommand(id, request.NewName));

                    return result.Match<Results<NoContent, NotFound<string>>>(
                        noContent => TypedResults.NoContent(),
                        notFoundError => TypedResults.NotFound(notFoundError.Message));
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record RenameWorkflowRequest(string NewName);

}

internal record RenameWorkflowCommand(Guid Id, string NewName) : ICommand<OneOf<Success, NotFoundError>>;

internal sealed class RenameWorkflowCommandHandler(IWorkflowRepository workflowRepository)
    : ICommandHandler<RenameWorkflowCommand, OneOf<Success, NotFoundError>>
{
    public async Task<OneOf<Success, NotFoundError>> Handle(RenameWorkflowCommand command,
        CancellationToken cancellationToken = default)

    {
        var workflow = await workflowRepository.GetById(command.Id);
        if (workflow is null)
        {
            return new NotFoundError($"Workflow {command.Id} not found.");
        }

        await workflowRepository.Rename(command.Id, command.NewName);
        return new Success();
    }
}