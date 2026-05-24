using System.Collections.Immutable;
using System.Text.Json;
using WfAssist.Executions.Contracts;
using WfAssist.Shared.Contracts;

namespace WfAssist.Executions.Core.Models;

internal sealed record Execution
{
    public required Guid Id { get; init; }
    public required ExecutionStatus Status { get; init; }
    public required ExecutionDataType DataType { get; init; }
    public required JsonDocument Data { get; init; }
    public ImmutableDictionary<string, ProcessingResult> ProcessingResults { get; init; } = [];
}



internal static class ExecutionFactory
{
    public static Execution CreateQueued(ExecutionDataType dataType, JsonDocument data)
    {
        return new Execution
        {
            Id = Guid.NewGuid(),
            Status = ExecutionStatus.Queued,
            DataType = dataType,
            Data = data
        };
    }
}