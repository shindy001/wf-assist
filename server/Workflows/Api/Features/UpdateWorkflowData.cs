using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using OneOf;
using OneOf.Types;
using WfAssist.Shared;
using WfAssist.Shared.CQRS;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class UpdateWorkflowData
{
    public static void MapUpdateWorkflowDataEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/updateData",
                async (Guid id, UpdateWorkflowDataRequest request, ICommandDispatcher commandDispatcher) =>
                {
                    var result =
                        await commandDispatcher.Dispatch(new UpdateWorkflowDataCommand(id, request.Data.ToDomain()));

                    return result.Match<Results<NoContent, NotFound<string>>>(
                        noContent => TypedResults.NoContent(),
                        notFoundError => TypedResults.NotFound(notFoundError.Message));
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record UpdateWorkflowDataRequest(WorkflowDataDto Data);

}

internal record UpdateWorkflowDataCommand(Guid Id, WorkflowData Data) : ICommand<OneOf<Success, NotFoundError>>;

internal sealed class UpdateWorkflowDataCommandHandler(IWorkflowRepository workflowRepository)
    : ICommandHandler<UpdateWorkflowDataCommand, OneOf<Success, NotFoundError>>
{
    public async Task<OneOf<Success, NotFoundError>> Handle(UpdateWorkflowDataCommand command,
        CancellationToken cancellationToken = default)

    {
        var workflow = await workflowRepository.GetById(command.Id);
        if (workflow is null)
        {
            return new NotFoundError($"Workflow {command.Id} not found.");
        }

        await workflowRepository.UpdateData(command.Id, command.Data);
        return new Success();
    }
}