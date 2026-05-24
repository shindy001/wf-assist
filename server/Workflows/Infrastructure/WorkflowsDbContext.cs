using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Infrastructure;

internal class WorkflowsDbContext : DbContext
{
    public DbSet<Workflow> Workflows { get; set; }

    private readonly JsonSerializerOptions _serializerOptions;

    public WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options, IOptions<JsonOptions> jsonOptions)
        : base(options)
    {
        _serializerOptions = jsonOptions.Value.SerializerOptions;
    }

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