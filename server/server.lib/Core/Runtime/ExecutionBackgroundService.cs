using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WfAssist.AspNetCore.Core.Runtime;

internal sealed partial class ExecutionBackgroundService : BackgroundService
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
        LogServiceIsStarting(nameof(ExecutionBackgroundService));

        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteNext();

            await Task.Delay(_checkInterval, stoppingToken);
        }

        LogServiceIsStopping(nameof(ExecutionBackgroundService));
    }

    private async Task ExecuteNext()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var executionManager = scope.ServiceProvider.GetRequiredService<ExecutionManager>();

        await executionManager.ExecuteNextInQueue();
    }

    [LoggerMessage(LogLevel.Information, "{service} is starting.")]
    partial void LogServiceIsStarting(string service);

    [LoggerMessage(LogLevel.Information, "{service} is stopping.")]
    partial void LogServiceIsStopping(string service);
}
