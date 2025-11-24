using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

public sealed partial class WorkflowExecutor
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
            LogExecutionStart(snapshot.Name);

            var executionOrder = WorkflowTopologySorter.CalculateNodeExecution(snapshot.Data);
            LogExecutionOrder(executionOrder.Select(x => x.Id));

            foreach (var node in executionOrder)
            {
                var dataType = node.Data.GetType();
                var processor = _serviceProvider.GetKeyedService<IWorkflowNodeProcessor>(nameof(RequestNodeData));

                if (processor is null)
                {
                    LogProcessorNotFoundError(dataType.Name);

                    return Task.FromResult(false);
                }

                processor.Process(node);
            }

            LogExecutionCompleted(snapshot.Name);
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            // TODO - return some error data with Result object
            return Task.FromResult(false);
        }
    }

    [LoggerMessage(LogLevel.Information, "Executing workflow '{workflowName}'.")]
    partial void LogExecutionStart(string workflowName);

    [LoggerMessage(LogLevel.Information, "Execution order: {executionOrder}")]
    partial void LogExecutionOrder(IEnumerable<string> executionOrder);

    [LoggerMessage(LogLevel.Error, """
                                   Processor for node data type {dataTypeName} was not found, ensure that processor is registered.
                                   Aborting workflow run.
                                   """)]
    partial void LogProcessorNotFoundError(string dataTypeName);

    [LoggerMessage(LogLevel.Information, "Workflow '{workflowName}' completed.")]
    partial void LogExecutionCompleted(string workflowName);
}
