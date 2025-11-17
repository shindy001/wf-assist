using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

public sealed class WorkflowExecutor
{
    private readonly ILogger<WorkflowExecutor> _logger;

    public WorkflowExecutor(ILogger<WorkflowExecutor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Execute specified <see cref="WorkflowSnapshot"/>.
    /// </summary>
    /// <param name="snapshot"><see cref="WorkflowSnapshot"/></param>
    /// <returns></returns>
    public Task Execute(WorkflowSnapshot snapshot)
    {
        var executionOrder = WorkflowTopologySorter.CalculateNodeExecution(snapshot.Data);

        _logger.LogInformation("Executing workflow '{workflowName}'.", snapshot.Name);
        _logger.LogInformation("Execution order: {executionOrder}", executionOrder.Select(x => x.Id));
        _logger.LogInformation("Workflow '{workflowName}' completed.", snapshot.Name);

        return Task.CompletedTask;
    }
}
