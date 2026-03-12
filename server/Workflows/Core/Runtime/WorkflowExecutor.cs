using Microsoft.Extensions.Logging;
using WfAssist.Workflows.Core.Models;
using WfAssist.Workflows.Core.Runtime.NodeProcessors;

namespace WfAssist.Workflows.Core.Runtime;

internal sealed partial class WorkflowExecutor
{
    private readonly WorkflowNodeProcessorProvider _processorProvider;
    private readonly ProcessingContext _processingContext;
    private readonly ILogger<WorkflowExecutor> _logger;

    public WorkflowExecutor(WorkflowNodeProcessorProvider processorProvider,
        ProcessingContext processingContext,
        ILogger<WorkflowExecutor> logger)
    {
        _processorProvider = processorProvider;
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
            var processor = _processorProvider.GetProcessor(node.Data);
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

    [LoggerMessage(LogLevel.Information, "Executing workflow '{workflowName}'.")]
    partial void LogExecutionStart(string workflowName);

    [LoggerMessage(LogLevel.Information, "Execution order: {executionOrder}")]
    partial void LogExecutionOrder(IEnumerable<string> executionOrder);

    [LoggerMessage(LogLevel.Information, "Workflow '{workflowName}' completed.")]
    partial void LogExecutionCompleted(string workflowName);

    [LoggerMessage(LogLevel.Error, "Workflow '{workflowName}' execution was interrupted. Reason: {reason}.")]
    partial void LogExecutionInterrupted(string workflowName, string reason);
}
