using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Core.Services;

internal interface IExecutionRepository
{
    Task<IEnumerable<ExecutionIdentity>> GetAll();
    Task<Execution?> GetById(Guid runId);
    Task<Execution?> GetQueued();
    Task<Guid> Add(Execution run);
    Task Complete(Guid runId, ExecutionStatus status, Dictionary<string, ProcessingResult> processingResults);
    Task UpdateStatus(Guid runId, ExecutionStatus status);
    Task Delete(Guid runId);
}