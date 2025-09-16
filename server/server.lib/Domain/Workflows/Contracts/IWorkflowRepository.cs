using WfAssist.AspNetCore.Domain.Workflows.Models;

namespace WfAssist.AspNetCore.Domain.Workflows.Contracts;

public interface IWorkflowRepository
{
    Task<IEnumerable<WorkflowIdentity>> GetIdentities();
    Task<Workflow?> GetById(Guid id);
    Task<bool> Exists(Guid workflowId);
    Task Create(Workflow workflow);
    Task Rename(Guid id, string newName);
    Task UpdateData(Guid workflowId, WorkflowData data);
    Task Delete(Guid id);
}

