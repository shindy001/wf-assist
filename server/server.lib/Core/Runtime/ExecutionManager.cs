using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Core.Models;
using WfAssist.AspNetCore.Core.Models.Notifications;
using WfAssist.AspNetCore.Core.Services;

namespace WfAssist.AspNetCore.Core.Runtime;

internal sealed partial class ExecutionManager
{
    private readonly IExecutionRepository _executionRepository;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly WorkflowExecutor _workflowExecutor;
    private readonly ProcessingContext _processingContext;
    private readonly ILogger<ExecutionManager> _logger;

    public ExecutionManager(
        IExecutionRepository executionRepository,
        INotificationDispatcher notificationDispatcher,
        WorkflowExecutor workflowExecutor,
        ProcessingContext processingContext,
        ILogger<ExecutionManager> logger)
    {
        _executionRepository = executionRepository;
        _notificationDispatcher = notificationDispatcher;
        _workflowExecutor = workflowExecutor;
        _processingContext = processingContext;
        _logger = logger;
    }

    /// <summary>
    /// Runs <see cref="Execution"/> specified by ID.
    /// </summary>
    /// <param name="executionId"></param>
    /// <exception cref="ArgumentException">When <see cref="Execution"/> with specified ID was not found.</exception>
    public async Task Execute(Guid executionId)
    {
        var execution = await _executionRepository.GetById(executionId);
        if (execution is null)
        {
            throw new ArgumentException($"Execution {executionId} could not be found.");
        }

        await Execute(execution);
    }

    /// <summary>
    /// Runs next queued <see cref="Execution"/> if there is any.
    /// </summary>
    /// <remarks>Does nothing if there is no queued <see cref="Execution"/>.</remarks>
    public async Task ExecuteNextInQueue()
    {
        var execution = await _executionRepository.GetQueued();
        if (execution is not null)
        {
            await Execute(execution);
        }
    }

    private async Task Execute(Execution execution)
    {
        try
        {
            await _executionRepository.UpdateStatus(execution.Id, ExecutionStatus.Running);

            LogExecutionStarted(execution.Id);
            await _notificationDispatcher.Dispatch(new WorkflowExecutionStarted
            {
                ExecutionId = execution.Id,
                WorkflowId = execution.Snapshot.Id,
                WorkflowName = execution.Snapshot.Name
            });

            await _workflowExecutor.Execute(execution.Snapshot);
            if (_processingContext.IsProcessingSuccessful())
            {
                await _executionRepository.Complete(execution.Id, ExecutionStatus.Completed,
                    _processingContext.ProcessingResults);

                LogExecutionCompleted(execution.Id);
                await _notificationDispatcher.Dispatch(new WorkflowExecutionEnded
                {
                    ExecutionId = execution.Id,
                    WorkflowId = execution.Snapshot.Id,
                    WorkflowName = execution.Snapshot.Name,
                    Status = ExecutionStatus.Completed
                });
            }
            else
            {
                await _executionRepository.Complete(execution.Id, ExecutionStatus.Failed,
                    _processingContext.ProcessingResults);

                LogExecutionCompletedWithErrors(execution.Id);
                await _notificationDispatcher.Dispatch(new WorkflowExecutionEnded
                {
                    ExecutionId = execution.Id,
                    WorkflowId = execution.Snapshot.Id,
                    WorkflowName = execution.Snapshot.Name,
                    Status = ExecutionStatus.Failed
                });
            }
        }
        catch (Exception e)
        {
            LogUnexpectedErrorDuringExecution(e.Message);

            _processingContext.AddResult("ProcessingError", ProcessingResult.Error(
                $"Unexpected error during execution: {e.Message}", string.Empty));

            await _executionRepository.Complete(execution.Id, ExecutionStatus.Failed,
                _processingContext.ProcessingResults);

            await _notificationDispatcher.Dispatch(new WorkflowExecutionEnded
            {
                ExecutionId = execution.Id,
                WorkflowId = execution.Snapshot.Id,
                WorkflowName = execution.Snapshot.Name,
                Status = ExecutionStatus.Failed
            });
        }
    }

    [LoggerMessage(LogLevel.Information, "Execution {runId} started.")]
    partial void LogExecutionStarted(Guid runId);

    [LoggerMessage(LogLevel.Information, "Execution {runId} completed.")]
    partial void LogExecutionCompleted(Guid runId);

    [LoggerMessage(LogLevel.Information, "Execution {runId} completed with errors.")]
    partial void LogExecutionCompletedWithErrors(Guid runId);

    [LoggerMessage(LogLevel.Error, "Unexpected error during execution. Error: {error}")]
    partial void LogUnexpectedErrorDuringExecution(string error);
}