namespace WfAssist.AspNetCore.Domain.Workflows.Models;

public abstract record WorkflowNodeData
{
    public abstract WorkflowNodeType Type { get; }
}

public sealed record PrintTextNodeData : WorkflowNodeData
{
    public override WorkflowNodeType Type => WorkflowNodeType.PrintText;

    public required string Text { get; init; }
    public bool UseConsole { get; init; }
    public string? TargetId { get; init; }
}

public sealed record RequestNodeData : WorkflowNodeData
{
    public override WorkflowNodeType Type => WorkflowNodeType.Request;

    public required string RequestType { get; init; }
    public required string Url { get; init; }
    public string? RequestBody { get; init; }
}

public sealed record ExtractPropertyNodeData : WorkflowNodeData
{
    public override WorkflowNodeType Type => WorkflowNodeType.ExtractProperty;

    public required string Path { get; init; }
    public required string TargetId { get; init; }
}