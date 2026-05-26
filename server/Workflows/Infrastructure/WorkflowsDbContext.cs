using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WfAssist.Shared;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Infrastructure;

internal class WorkflowsDbContext : DbContext, IUnitOfWork, IRepository<Workflow>
{
    public DbSet<Workflow> Workflows { get; set; }

    private readonly JsonSerializerOptions _serializerOptions;

    public WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options, IOptions<JsonOptions> jsonOptions)
        : base(options)
    {
        _serializerOptions = jsonOptions.Value.SerializerOptions;
    }

    public IRepository<TAggregate> GetRepository<TAggregate>() => (IRepository<TAggregate>) this;

    public async Task<Workflow?> TryFindAsync(Guid key) => await Workflows.FindAsync([key]);
    public void Add(Workflow aggregate) => Workflows.Add(aggregate);
    public void Delete(Workflow aggregate) => Workflows.Remove(aggregate);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workflow>()
            .Property(x => x.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _serializerOptions),
                v => JsonSerializer.Deserialize<WorkflowData>(v, _serializerOptions)!);

        base.OnModelCreating(modelBuilder);
    }
}