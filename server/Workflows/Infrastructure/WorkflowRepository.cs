using Microsoft.EntityFrameworkCore;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Services;

namespace WfAssist.Workflows.Infrastructure;

internal sealed class WorkflowRepository(WorkflowsDbContext dbContext) : IWorkflowRepository
{
    public async Task<IEnumerable<WorkflowIdentity>> GetIdentities()
    {
        return await dbContext.Workflows.Select(x => new WorkflowIdentity(x.Id, x.Name)).ToListAsync();
    }

    public async Task<Workflow?> GetById(Guid id)
    {
        return await dbContext.Workflows.FindAsync(id);
    }

    public async Task Create(Workflow workflow)
    {
        await dbContext.Workflows.AddAsync(workflow);
        await dbContext.SaveChangesAsync();
    }

    public async Task Rename(Guid id, string newName)
    {
        var item = await dbContext.Workflows.FindAsync(id);
        if (item is null)
        {
            // TODO - custom exception that will be translated to 400 in aspnetcore global exception handler?
            throw new InvalidOperationException($"Workflow with {id} not found.");
        }

        dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
        {
            { nameof(Workflow.Name), newName }
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateData(Guid id, WorkflowData data)
    {
        var item = await dbContext.Workflows.FindAsync(id);
        if (item is null)
        {
            // TODO - custom exception that will be translated to 400 in aspnetcore global exception handler?
            throw new InvalidOperationException($"Workflow with {id} not found.");
        }

        dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
        {
            { nameof(Workflow.Data), data }
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var item = await dbContext.Workflows.FindAsync(id);
        if (item is not null)
        {
            dbContext.Workflows.Remove(item);
            await dbContext.SaveChangesAsync();
        }
    }
}