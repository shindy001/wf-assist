using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using WfAssist.Executions.Core.Models;
using WfAssist.Executions.Infrastructure;
using WfAssist.Shared.Contracts;
using WfAssist.Shared.CQRS;

namespace WfAssist.Executions.Api.Features;

internal static class GetWorkflowExecutionById
{
    internal static void MapGetWorkflowExecutionByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/{id:guid}", async (Guid id, IQueryDispatcher queryDispatcher) =>
            {
                var execution = await queryDispatcher.Dispatch(new GetExecutionByIdQuery(id));
                return execution is null
                    ? Results.NotFound()
                    : Results.Ok(new GetWorkflowExecutionByIdResponse
                    {
                        Id = execution.Id,
                        Status = execution.Status,
                        DataSnapshot = execution.Data,
                        ProcessingResults = execution.ProcessingResults
                    });
            })
            .Produces<GetWorkflowExecutionByIdResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private sealed record GetWorkflowExecutionByIdResponse
    {
        public required Guid Id { get; init; }
        public required ExecutionStatus Status { get; init; }
        public required JsonDocument DataSnapshot { get; init; }
        public ImmutableDictionary<string, ProcessingResult> ProcessingResults { get; init; } = [];
    }
}

internal record GetExecutionByIdQuery(Guid ExecutionId) : IQuery<Execution?>;

internal sealed class GetWorkflowExecutionByIdQueryHandler(ExecutionsDbContext dbContext)
    : IQueryHandler<GetExecutionByIdQuery, Execution?>
{
    public async Task<Execution?> Handle(GetExecutionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Executions.FindAsync([query.ExecutionId], cancellationToken: cancellationToken);
    }
}