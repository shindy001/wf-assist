using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Contracts;

public interface IWorkflowRepository
{
    Task<IEnumerable<WorkflowIdentity>> GetIdentities();
    Task<Workflow?> GetById(Guid id);
    Task<bool> Exists(Guid id);
    Task Create(Workflow workflow);
    Task Rename(Guid id, string newName);
    Task UpdateData(Guid id, WorkflowData data);
    Task Delete(Guid id);
}

