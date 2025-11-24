using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

public sealed class WorkflowExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WorkflowExecutor> _logger;

    public WorkflowExecutor(IServiceProvider serviceProvider, ILogger<WorkflowExecutor> logger)
    {
        _serviceProvider = serviceProvider;
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
            _logger.LogInformation("Executing workflow '{workflowName}'.", snapshot.Name);

            var executionOrder = WorkflowTopologySorter.CalculateNodeExecution(snapshot.Data);
            _logger.LogInformation("Execution order: {executionOrder}", executionOrder.Select(x => x.Id));

            foreach (var node in executionOrder)
            {
                var dataType = node.Data.GetType();
                var processor = _serviceProvider.GetKeyedService<IWorkflowNodeProcessor>(nameof(RequestNodeData));

                if (processor is null)
                {
                    _logger.LogError(
                        """
                        Processor for node data type {dataTypeName} was not found, ensure that processor is registered.
                        Aborting workflow run.
                        """, dataType.Name);

                    return Task.FromResult(false);
                }

                processor.Process(node);
            }

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
