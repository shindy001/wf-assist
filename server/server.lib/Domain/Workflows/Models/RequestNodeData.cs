namespace WfAssist.AspNetCore.Domain.Workflows.Models;

public sealed record RequestNodeData() : WorkflowNodeDataBase(WorkflowNodeType.Request)
{
    public required string RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}