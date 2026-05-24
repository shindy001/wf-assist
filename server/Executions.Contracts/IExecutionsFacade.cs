using System.Collections.Immutable;
using WfAssist.Shared.Contracts;

namespace WfAssist.Executions.Contracts;

public interface IExecutionsFacade
{
	Task<TData?> GetQueued<TData>(Guid executionId, ExecutionDataType dataType);
	Task<(Guid executionId, TData data)?> GetNextQueued<TData>(ExecutionDataType dataType);
	Task<Guid> Queue<T>(ExecutionDataType dataType, T data);

	Task MarkAsRunning(Guid executionId);
	Task Complete(Guid executionId, ImmutableDictionary<string, ProcessingResult> processingResults);
	Task Fail(Guid executionId, ImmutableDictionary<string, ProcessingResult> processingResults);
}