using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneOf.Types;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

internal sealed partial class WorkflowExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WorkflowExecutor> _logger;

    public WorkflowExecutor(IServiceProvider serviceProvider,
        ILogger<WorkflowExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Execute specified <see cref="WorkflowSnapshot"/>.
    /// </summary>
    /// <param name="snapshot"><see cref="WorkflowSnapshot"/></param>
    public async Task Execute(WorkflowSnapshot snapshot)
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
                LogExecutionInterrupted(snapshot.Name,
                    $"Processor for node data type {dataType.Name} was not found, ensure that processor is registered.");
                return;
            }

            var result = await processor.Process(node);

            if (result.Value is Error)
            {
                LogExecutionInterrupted(snapshot.Name, $"Error during processing of node {node.Id}.");
                return;
            }
        }

        LogExecutionCompleted(snapshot.Name);
    }

    [LoggerMessage(LogLevel.Information, "Executing workflow '{workflowName}'.")]
    partial void LogExecutionStart(string workflowName);

    [LoggerMessage(LogLevel.Information, "Execution order: {executionOrder}")]
    partial void LogExecutionOrder(IEnumerable<string> executionOrder);

    [LoggerMessage(LogLevel.Information, "Workflow '{workflowName}' completed.")]
    partial void LogExecutionCompleted(string workflowName);

    [LoggerMessage(LogLevel.Error, "Workflow '{workflowName}' execution was interrupted. Reason: {reason}.")]
    partial void LogExecutionInterrupted(string workflowName, string reason);

}
