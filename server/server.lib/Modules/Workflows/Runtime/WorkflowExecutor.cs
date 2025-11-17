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
    /// <returns>True is execute was successful</returns>
    // TODO - replace bool return with OneOf or Result object???
    public Task<bool> Execute(WorkflowSnapshot snapshot)
    {
        try
        {
            var executionOrder = WorkflowTopologySorter.CalculateNodeExecution(snapshot.Data);

            _logger.LogInformation("Executing workflow '{workflowName}'.", snapshot.Name);
            _logger.LogInformation("Execution order: {executionOrder}", executionOrder.Select(x => x.Id));
            _logger.LogInformation("Workflow '{workflowName}' completed.", snapshot.Name);
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            // TODO - return some error data with Result object
            return Task.FromResult(false);
        }
    }
}
