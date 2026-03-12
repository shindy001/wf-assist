using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Core.Runtime.NodeProcessors;

public interface IWorkflowNodeProcessor
{
    Task<ProcessingResult> Process(WorkflowNode workflowNode);
}