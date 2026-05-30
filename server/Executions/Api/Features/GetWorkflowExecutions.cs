using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using WfAssist.Executions.Contracts;
using WfAssist.Executions.Core.Models;
using WfAssist.Executions.Infrastructure;
using WfAssist.Shared.Contracts;
using WfAssist.Shared.CQRS;

namespace WfAssist.Executions.Api.Features;

public static class GetWorkflowExecutions
{
    public static void MapGetWorkflowExecutionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", async (IQueryDispatcher queryDispatcher) =>
            {
                var executions = await queryDispatcher.Dispatch(new GetExecutionsQuery());
                var response = new GetWorkflowExecutionsResponse
                {
                    Items = executions.Select(x => new GetWorkflowExecutionsResponse.WorkflowExecutionDto
                    {
                        Id = x.Id,
                        Status = x.Status,
                        DataSnapshot = x.Data,
                        ProcessingResults = x.ProcessingResults
                    })
                };

                return TypedResults.Ok(response);
            })
            .Produces<GetWorkflowExecutionsResponse>();
    }

    private sealed record GetWorkflowExecutionsResponse
    {
        public required IEnumerable<WorkflowExecutionDto> Items { get; init; } = [];

        internal sealed class WorkflowExecutionDto
        {
            public required Guid Id { get; init; }
            public required ExecutionStatus Status { get; init; }
            public required JsonDocument DataSnapshot { get; init; }
            public ImmutableDictionary<string, ProcessingResult> ProcessingResults { get; init; } = [];
        }
    }
}

internal record GetExecutionsQuery : IQuery<IEnumerable<Execution>>;

internal sealed class GetWorkflowExecutionsQueryHandler(ExecutionsDbContext dbContext)
    : IQueryHandler<GetExecutionsQuery, IEnumerable<Execution>>
{
    public async Task<IEnumerable<Execution>> Handle(GetExecutionsQuery query,
        CancellationToken cancellationToken = default)
    {
        // TODO add pagination
        return await dbContext.Executions.Where(x => x.DataType == ExecutionDataType.Workflow)
            .ToListAsync(cancellationToken);
    }
}