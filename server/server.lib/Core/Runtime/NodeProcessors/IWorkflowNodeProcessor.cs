using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Core.Runtime.NodeProcessors;

public interface IWorkflowNodeProcessor
{
    Task<ProcessingResult> Process(WorkflowNode workflowNode);
}