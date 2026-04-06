using OneOf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Shared;
using Shared.CQRS;
using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Api.Mappers;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Api.Features;

public static class GetWorkflowById
{
    public static void MapGetWorkflowByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/{id:guid}", async (Guid id, IQueryDispatcher queryDispatcher) =>
            {
                var result = await queryDispatcher.Dispatch(new GetWorkflowByIdQuery(id));

                return result.Match<Results<Ok<GetWorkflowByIdResponse>, NotFound<string>>>(
                    workflow => TypedResults.Ok(new GetWorkflowByIdResponse(workflow.ToDto())),
                    notFoundError => TypedResults.NotFound(notFoundError.Message));

            })
            .Produces<GetWorkflowByIdResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record GetWorkflowByIdResponse(WorkflowDto Item);

}

internal record GetWorkflowByIdQuery(Guid Id) : IQuery<OneOf<Workflow, NotFoundError>>;

internal sealed class GetWorkflowByIdQueryHandler(IWorkflowRepository workflowRepository)
    : IQueryHandler<GetWorkflowByIdQuery, OneOf<Workflow, NotFoundError>>
{
    public async Task<OneOf<Workflow, NotFoundError>> Handle(GetWorkflowByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var workflow = await workflowRepository.GetById(query.Id);
        return workflow is null
            ? new NotFoundError($"Workflow with id '{query.Id}' not found")
            : workflow;
    }
}