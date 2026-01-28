using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Core.Services;

internal interface IWorkflowRepository
{
    Task<IEnumerable<WorkflowIdentity>> GetIdentities();
    Task<Workflow?> GetById(Guid id);
    Task<bool> Exists(Guid id);
    Task Create(Workflow workflow);
    Task Rename(Guid id, string newName);
    Task UpdateData(Guid id, WorkflowData data);
    Task Delete(Guid id);
}