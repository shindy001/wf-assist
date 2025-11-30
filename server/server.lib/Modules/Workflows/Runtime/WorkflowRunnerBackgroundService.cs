using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

internal sealed class WorkflowRunnerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<WorkflowRunnerBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);

    public WorkflowRunnerBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<WorkflowRunnerBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{service} is starting.", nameof(WorkflowRunnerBackgroundService));

        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteQueuedWorkflow();

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("{service} is stopping.", nameof(WorkflowRunnerBackgroundService));
    }

    private async Task ExecuteQueuedWorkflow()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var workflowProcessingRepository = scope.ServiceProvider.GetRequiredService<WorkflowProcessingRepository>();
        var workflowExecutor = scope.ServiceProvider.GetRequiredService<WorkflowExecutor>();
        var processingContext = scope.ServiceProvider.GetRequiredService<ProcessingContext>();
        WorkflowRun? workflowRun = null;

        try
        {
            workflowRun = await workflowProcessingRepository.GetQueuedRun();

            if (workflowRun is not null)
            {
                await workflowProcessingRepository.UpdateRunStatus(workflowRun.Id, WorkflowRunStatus.Running);
                _logger.LogInformation("Starting workflow run {runId}.", workflowRun.Id);

                await workflowExecutor.Execute(workflowRun.Snapshot);
                if (processingContext.IsProcessingSuccessful())
                {
                    await workflowProcessingRepository.CompleteRun(workflowRun.Id, WorkflowRunStatus.Completed,
                        processingContext.ProcessingResults.Values.ToList());
                    _logger.LogInformation("Workflow run {runId} completed.", workflowRun.Id);
                }
                else
                {
                    await workflowProcessingRepository.CompleteRun(workflowRun.Id, WorkflowRunStatus.Failed,
                        processingContext.ProcessingResults.Values.ToList());
                    _logger.LogInformation("Workflow run {runId} completed with errors.", workflowRun.Id);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while execution workflow run.");

            if (workflowRun is not null)
            {
                processingContext.AddResult("ProcessingError", ProcessingResult.Error(
                    $"Unexpected error during workflow run: {e.Message}", string.Empty));

                await workflowProcessingRepository.CompleteRun(workflowRun.Id, WorkflowRunStatus.Failed,
                    processingContext.ProcessingResults.Values.ToList());
            }
        }
    }
}
