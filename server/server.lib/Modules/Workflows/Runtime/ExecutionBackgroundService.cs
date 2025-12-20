using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models.Notifications;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure;
using ExecutionStatus = WfAssist.AspNetCore.Modules.Workflows.Domain.Models.ExecutionStatus;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

internal sealed class ExecutionBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ExecutionBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);

    public ExecutionBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<ExecutionBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{service} is starting.", nameof(ExecutionBackgroundService));

        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteQueuedWorkflow();

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("{service} is stopping.", nameof(ExecutionBackgroundService));
    }

    private async Task ExecuteQueuedWorkflow()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var workflowProcessingRepository = scope.ServiceProvider.GetRequiredService<ExecutionRepository>();
        var workflowExecutor = scope.ServiceProvider.GetRequiredService<WorkflowExecutor>();
        var processingContext = scope.ServiceProvider.GetRequiredService<ProcessingContext>();
        var notificationDispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();
        Execution? execution = null;

        try
        {
            execution = await workflowProcessingRepository.GetQueuedRun();

            if (execution is not null)
            {
                await workflowProcessingRepository.UpdateRunStatus(execution.Id, ExecutionStatus.Running);

                _logger.LogInformation("Execution {runId} started.", execution.Id);
                await notificationDispatcher.Dispatch(new ExecutionStarted
                    {ExecutionId = execution.Id, WorkflowId = execution.Snapshot.Id});

                await workflowExecutor.Execute(execution.Snapshot);
                if (processingContext.IsProcessingSuccessful())
                {
                    await workflowProcessingRepository.CompleteRun(execution.Id, ExecutionStatus.Completed,
                        processingContext.ProcessingResults);

                    _logger.LogInformation("Execution {runId} completed.", execution.Id);
                    await notificationDispatcher.Dispatch(new ExecutionEnded
                    {
                        ExecutionId = execution.Id, WorkflowId = execution.Snapshot.Id,
                        Status = Domain.Models.Notifications.ExecutionStatus.Completed
                    });
                }
                else
                {
                    await workflowProcessingRepository.CompleteRun(execution.Id, ExecutionStatus.Failed,
                        processingContext.ProcessingResults);

                    _logger.LogInformation("Execution {runId} completed with errors.", execution.Id);
                    await notificationDispatcher.Dispatch(new ExecutionEnded
                    {
                        ExecutionId = execution.Id, WorkflowId = execution.Snapshot.Id,
                        Status = Domain.Models.Notifications.ExecutionStatus.Failed
                    });
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while during execution.");

            if (execution is not null)
            {
                processingContext.AddResult("ProcessingError", ProcessingResult.Error(
                    $"Unexpected error during execution: {e.Message}", string.Empty));

                await workflowProcessingRepository.CompleteRun(execution.Id, ExecutionStatus.Failed,
                    processingContext.ProcessingResults);

                await notificationDispatcher.Dispatch(new ExecutionEnded
                {
                    ExecutionId = execution.Id, WorkflowId = execution.Snapshot.Id,
                    Status = Domain.Models.Notifications.ExecutionStatus.Failed
                });
            }
        }
    }
}
