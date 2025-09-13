namespace WfAssist.AspNetCore.Domain.Workflows.Models;

/// <summary>
/// Marker interface of node data.
/// </summary>
public interface IWorkflowNodeData;

public abstract record WorkflowNodeDataBase(WorkflowNodeType Type) : IWorkflowNodeData;