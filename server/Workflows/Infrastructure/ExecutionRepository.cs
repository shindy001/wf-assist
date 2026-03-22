using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Infrastructure;

internal sealed class ExecutionRepository(WorkflowsDbContext dbContext) : IExecutionRepository
{
    public async Task<IEnumerable<ExecutionIdentity>> GetAll()
    {
        return await dbContext.Executions.Select(x => new ExecutionIdentity {Id = x.Id, Status = x.Status})
            .ToListAsync();
    }

    public async Task<Execution?> GetById(Guid runId)
    {
        return await dbContext.Executions.FindAsync(runId);
    }

    public async Task<Execution?> GetQueued()
    {
        return await dbContext.Executions.FirstOrDefaultAsync(x => x.Status == ExecutionStatus.Queued);
    }

    public async Task<Guid> Add(Execution run)
    {
        await dbContext.Executions.AddAsync(run);
        await dbContext.SaveChangesAsync();

        return run.Id;
    }

    public async Task Complete(Guid runId, ExecutionStatus status, ImmutableDictionary<string, ProcessingResult> processingResults)
    {
        var item = await dbContext.Executions.FindAsync(runId);
        if (item is null)
        {
            // TODO - custom exception that will be translated to 400 in aspnetcore global exception handler?
            throw new InvalidOperationException($"Execution with ID '{runId}' does not exist");
        }

        dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
        {
            { nameof(Execution.Status), status },
            { nameof(Execution.ProcessingResults), processingResults }
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateStatus(Guid runId, ExecutionStatus status)
    {
        var item = await dbContext.Executions.FindAsync(runId);
        if (item is null)
        {
            // TODO - custom exception that will be translated to 400 in aspnetcore global exception handler?
            throw new InvalidOperationException($"Execution with ID '{runId}' does not exist");
        }

        dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
        {
            { nameof(Execution.Status), status }
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid runId)
    {
        var item = await dbContext.Executions.FindAsync(runId);
        if (item is not null)
        {
            dbContext.Executions.Remove(item);
            await dbContext.SaveChangesAsync();
        }
    }
}