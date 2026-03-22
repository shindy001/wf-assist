using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shared;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Infrastructure;

public class WorkflowsDbContext : DbContext
{
    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<Execution> Executions { get; set; }

    private readonly JsonSerializerOptions _serializerOptions = JsonDefaults.SerializerOptions;

    public WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workflow>()
            .Property(x => x.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _serializerOptions),
                v => JsonSerializer.Deserialize<WorkflowData>(v, _serializerOptions)!);

        modelBuilder.Entity<Execution>()
            .Property(x => x.Snapshot)
            .HasConversion(
            v => JsonSerializer.Serialize(v),
            v => JsonSerializer.Deserialize<WorkflowSnapshot>(v)!);

        modelBuilder.Entity<Execution>()
            .Property(x => x.ProcessingResults)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _serializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, ProcessingResult>>(v, _serializerOptions)!);

        base.OnModelCreating(modelBuilder);
    }
}