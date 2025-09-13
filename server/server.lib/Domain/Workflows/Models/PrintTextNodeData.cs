namespace WfAssist.AspNetCore.Domain.Workflows.Models;

public sealed record PrintTextNodeData() : WorkflowNodeDataBase(WorkflowNodeType.PrintText)
{
    public required string Text { get; init; }
    public bool UseConsole { get; init; }
    public string? TargetId { get; init; }
}