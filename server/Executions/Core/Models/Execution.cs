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
