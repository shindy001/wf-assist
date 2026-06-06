using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using WfAssist.Executions.Contracts;
using WfAssist.Shared.Contracts;
using WfAssist.Shared.Notifications;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Models.Notifications;

namespace WfAssist.Workflows.Core.Runtime;

internal sealed partial class ExecutionManager
{
    private readonly IExecutionsFacade _executionsFacade;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly WorkflowExecutor _workflowExecutor;
    private readonly ProcessingContext _processingContext;
    private readonly ILogger<ExecutionManager> _logger;

    public ExecutionManager(
        IExecutionsFacade executionsFacade,
        INotificationDispatcher notificationDispatcher,
        WorkflowExecutor workflowExecutor,
        ProcessingContext processingContext,
        ILogger<ExecutionManager> logger)
    {
        _executionsFacade = executionsFacade;
        _notificationDispatcher = notificationDispatcher;
        _workflowExecutor = workflowExecutor;
        _processingContext = processingContext;
        _logger = logger;
    }

    /// <summary>
    /// Runs queued <see cref="Workflow"/>.
    /// </summary>
    /// <param name="executionId">ID of the execution containing Workflow data.</param>
    /// <exception cref="ArgumentException">When queued Workflow with specified execution ID was not found.</exception>
    public async Task Execute(Guid executionId)
    {
        var data = await _executionsFacade.GetQueued<Workflow>(executionId, ExecutionDataType.Workflow);
        if (data is null)
        {
            throw new ArgumentException($"Data for Execution '{executionId}' not be found.");
        }

        await Execute(executionId, data);
    }

    /// <summary>
    /// Runs next queued Workflow execution.
    /// </summary>
    /// <remarks>Does nothing if there is no queued Workflow.</remarks>
    public async Task ExecuteNextInQueue()
    {
        var next = await _executionsFacade.GetNextQueued<Workflow>(ExecutionDataType.Workflow);
        if (next is not null)
        {
            await Execute(next.Value.executionId, next.Value.data);
        }
    }

    private async Task Execute(Guid executionId, Workflow workflow)
    {
        try
        {
            LogExecutionStarted(executionId);
            await NotifyWorkflowExecutionStart(executionId, workflow);
            await _executionsFacade.MarkAsRunning(executionId);

            await _workflowExecutor.Execute(workflow);

            if (_processingContext.IsProcessingSuccessful())
            {
                LogExecutionCompleted(executionId);
                await _executionsFacade.Complete(executionId, _processingContext.ProcessingResults.ToImmutableDictionary());
                await NotifyWorkflowExecutionEnd(executionId, workflow, "Completed");
            }
            else
            {
                LogExecutionCompletedWithErrors(executionId);
                await _executionsFacade.Fail(executionId, _processingContext.ProcessingResults.ToImmutableDictionary());
                await NotifyWorkflowExecutionEnd(executionId, workflow, "Failed");
            }
        }
        catch (Exception e)
        {
            LogUnexpectedErrorDuringExecution(e.Message);

            _processingContext.AddResult("ProcessingError", ProcessingResult.Error(
                $"Unexpected error during execution: {e.Message}"));

            await _executionsFacade.Fail(executionId, _processingContext.ProcessingResults.ToImmutableDictionary());
            await NotifyWorkflowExecutionEnd(executionId, workflow, "Failed");
        }
    }

    private async Task NotifyWorkflowExecutionEnd(Guid executionId, Workflow workflow, string status)
    {
        await _notificationDispatcher.Dispatch(new WorkflowExecutionEnded
        {
            ExecutionId = executionId,
            WorkflowId = workflow.Id,
            WorkflowName = workflow.Name,
            Status = status
        });
    }

    private async Task NotifyWorkflowExecutionStart(Guid executionId, Workflow workflow)
    {
        await _notificationDispatcher.Dispatch(new WorkflowExecutionStarted
        {
            ExecutionId = executionId,
            WorkflowId = workflow.Id,
            WorkflowName = workflow.Name
        });
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