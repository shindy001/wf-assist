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
    /// Execute specified <see cref="Workflow"/>.
    /// </summary>
    /// <param name="workflow">Workflow to execute</param>
    public async Task Execute(Workflow workflow)
    {
        LogExecutionStart(workflow.Name);

        var executionOrder = WorkflowTopologySorter.CalculateNodeExecution(workflow.Data);
        LogExecutionOrder(executionOrder.Select(x => x.Id));

        foreach (var node in executionOrder)
        {
            var processor = _processorProvider.GetProcessor(node.Data);
            var result = await processor.Process(node);

            _processingContext.AddResult(node.Id, result);

            if (!result.IsSuccessful)
            {
                LogExecutionInterrupted(workflow.Name, $"Error during processing of node {node.Id}.");
                return;
            }
        }

        LogExecutionCompleted(workflow.Name);
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
