using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Core.Runtime.NodeProcessors;

internal interface IWorkflowNodeProcessor
{
    Task<ProcessingResult> Process(WorkflowNode workflowNode);
}