using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

internal sealed partial class WorkflowExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ProcessingContext _processingContext;
    private readonly ILogger<WorkflowExecutor> _logger;

    public WorkflowExecutor(IServiceProvider serviceProvider,
        ProcessingContext processingContext,
        ILogger<WorkflowExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _processingContext = processingContext;
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
            var processorServiceKey = GetProcessorServiceKey(node.Data);
            var processor = _serviceProvider.GetKeyedService<IWorkflowNodeProcessor>(processorServiceKey);

            if (processor is null)
            {
                LogExecutionInterrupted(snapshot.Name,
                    $"Workflow Processor with service key {processorServiceKey} for node data type {node.Data.GetType().Name} " +
                    $"was not found, ensure that processor is registered.");
                return;
            }

            var result = await processor.Process(node);
            _processingContext.AddResult(node.Id, result);

            if (!result.IsSuccessful)
            {
                LogExecutionInterrupted(snapshot.Name, $"Error during processing of node {node.Id}.");
                return;
            }
        }

        LogExecutionCompleted(snapshot.Name);
    }

    private static string GetProcessorServiceKey(WorkflowNodeData nodeData)
    {
        return nodeData switch
        {
            RequestNodeData => WorkflowConstants.RequestNodeProcessorKey,
            HeadersNodeData => WorkflowConstants.HeadersNodeProcessorKey,
            _ => throw new InvalidOperationException($"Unknown node data type {nodeData.GetType().Name}")
        };
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
