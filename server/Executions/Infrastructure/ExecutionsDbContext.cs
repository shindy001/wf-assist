using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using WfAssist.Executions.Contracts;
using WfAssist.Executions.Core.Models;
using WfAssist.Shared.Contracts;

namespace WfAssist.Executions.Infrastructure;

internal sealed class ExecutionsDbContext : DbContext
{
    public DbSet<Execution> Executions { get; set; }

    private readonly JsonSerializerOptions _serializerOptions;

    public ExecutionsDbContext(DbContextOptions<ExecutionsDbContext> options, IOptions<JsonOptions> jsonOptions)
        : base(options)
    {
        _serializerOptions = jsonOptions.Value.SerializerOptions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Execution>()
            .Property(x => x.Data)
            .HasConversion(
            v => v.RootElement.GetRawText(),
            v => JsonDocument.Parse(v));

        modelBuilder.Entity<Execution>()
            .Property(x => x.ProcessingResults)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _serializerOptions),
                v => JsonSerializer.Deserialize<ImmutableDictionary<string, ProcessingResult>>(v, _serializerOptions)!,
                new ValueComparer<ImmutableDictionary<string, ProcessingResult>>(
                    (d1, d2) => ReferenceEquals(d1, d2),
                    d => d.GetHashCode(),
                    d => d));

        base.OnModelCreating(modelBuilder);
    }
}