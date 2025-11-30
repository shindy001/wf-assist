using OneOf;
using OneOf.Types;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

public interface IWorkflowNodeProcessor
{
    Task<OneOf<Success, Error>> Process(WorkflowNode workflowNode);
}