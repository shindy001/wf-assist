using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

public class WorkflowRunnerBackgroundService : BackgroundService
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
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var workflowProcessingRepository =
                    scope.ServiceProvider.GetRequiredService<WorkflowProcessingRepository>();
                var workflowExecutor = scope.ServiceProvider.GetRequiredService<WorkflowExecutor>();

                var workflowRun = await workflowProcessingRepository.GetQueuedRun();
                if (workflowRun is not null)
                {
                    _logger.LogInformation("Starting workflow run {runId}.", workflowRun.Id);
                    await workflowExecutor.Execute(workflowRun.Snapshot);
                    await workflowProcessingRepository.CompleteRun(workflowRun.Id);
                    _logger.LogInformation("Workflow run {runId} completed.", workflowRun.Id);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while execution workflow run.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("{service} is stopping.", nameof(WorkflowRunnerBackgroundService));
    }
}