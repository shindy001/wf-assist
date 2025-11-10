using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Contracts;

public interface IWorkflowProcessingRepository
{
    Task<IEnumerable<WorkflowRunIdentity>> GetAllRuns();
    Task<WorkflowRun?> GetById(Guid runId);
    Task Delete(Guid runId);

    Task<Guid> QueueRun(WorkflowRun run);
}

