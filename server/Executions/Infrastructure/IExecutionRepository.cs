using System.Collections.Immutable;
using WfAssist.Executions.Contracts;
using WfAssist.Executions.Core.Models;
using WfAssist.Shared.Contracts;

namespace WfAssist.Executions.Infrastructure;

internal interface IExecutionRepository
{
    Task<IEnumerable<ExecutionIdentity>> GetAll();
    Task<Execution?> GetById(Guid runId);
    Task<Execution?> GetQueued();
    Task<Guid> Add(Execution run);
    Task Complete(Guid runId, ExecutionStatus status, ImmutableDictionary<string, ProcessingResult> processingResults);
    Task UpdateStatus(Guid runId, ExecutionStatus status);
    Task Delete(Guid runId);
}