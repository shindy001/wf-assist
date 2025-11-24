using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

public interface IWorkflowNodeProcessor
{
    Task<ProcessResult> Process(WorkflowNode workflowNode);
}