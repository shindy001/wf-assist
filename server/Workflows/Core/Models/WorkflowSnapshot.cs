namespace WfAssist.Workflows.Core.Models;

internal sealed record WorkflowSnapshot
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowData Data { get; init; }
}