using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WfAssist.Executions.Contracts;
using WfAssist.Executions.Core.Models;
using WfAssist.Shared.Contracts;

namespace WfAssist.Executions.Infrastructure;

internal sealed class ExecutionsFacade(ExecutionsDbContext dbContext, IOptions<JsonOptions> jsonOptions) : IExecutionsFacade
{
	public async Task<TData?> GetQueued<TData>(Guid executionId, ExecutionDataType dataType)
	{
		var result =
			await dbContext.Executions.FirstOrDefaultAsync(x =>
				x.Id == executionId && x.Status == ExecutionStatus.Queued);
		return result is null
			? default
			: result.Data.Deserialize<TData>(jsonOptions.Value.SerializerOptions);
	}

	public async Task<(Guid executionId, TData data)?> GetNextQueued<TData>(ExecutionDataType dataType)
	{
		var result = await dbContext.Executions.FirstOrDefaultAsync(x => x.Status == ExecutionStatus.Queued);
		return result is null
			? null
			: (result.Id, result.Data.Deserialize<TData>(jsonOptions.Value.SerializerOptions)!);
	}

	public async Task<Guid> Queue<T>(ExecutionDataType dataType, T data)
	{
		var jsonDocData = JsonSerializer.SerializeToDocument(data);
		var execution = new Execution
		{
			Id = Guid.NewGuid(),
			Status = ExecutionStatus.Queued,
			DataType = dataType,
			Data = jsonDocData,
		};
		await dbContext.Executions.AddAsync(execution);
		await dbContext.SaveChangesAsync();

		return execution.Id;
	}

	public async Task MarkAsRunning(Guid executionId)
	{
		var item = await dbContext.Executions.FindAsync(executionId);
		if (item is null)
		{
			// TODO - return Maybe<Error>???
			throw new InvalidOperationException($"Execution with ID '{executionId}' does not exist");
		}

		dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
		{
			{ nameof(Execution.Status), ExecutionStatus.Running }
		});

		await dbContext.SaveChangesAsync();
	}

	public async Task Complete(Guid executionId, ImmutableDictionary<string, ProcessingResult> processingResults)
	{
		var item = await dbContext.Executions.FindAsync(executionId);
		if (item is null)
		{
			// TODO - return Maybe<Error>???
			throw new InvalidOperationException($"Execution with ID '{executionId}' does not exist");
		}

		dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
		{
			{ nameof(Execution.Status), ExecutionStatus.Completed },
			{ nameof(Execution.ProcessingResults), processingResults }
		});

		await dbContext.SaveChangesAsync();
	}

	public async Task Fail(Guid executionId, ImmutableDictionary<string, ProcessingResult> processingResults)
	{
		var item = await dbContext.Executions.FindAsync(executionId);
		if (item is null)
		{
			// TODO - return Maybe<Error>???
			throw new InvalidOperationException($"Execution with ID '{executionId}' does not exist");
		}

		dbContext.Entry(item).CurrentValues.SetValues(new Dictionary<string, object>
		{
			{ nameof(Execution.Status), ExecutionStatus.Failed },
			{ nameof(Execution.ProcessingResults), processingResults }
		});

		await dbContext.SaveChangesAsync();
	}
}