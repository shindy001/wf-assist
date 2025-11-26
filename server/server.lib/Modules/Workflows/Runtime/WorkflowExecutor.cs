using System.Collections.Immutable;
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
    /// <returns>Bool to specify if run was successful and results of the run.</returns>
    public async Task<(bool success, ImmutableArray<ProcessingResult> results)> Execute(WorkflowSnapshot snapshot)
    {
        var processResults = new List<ProcessingResult>();

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

                    return (false, [..processResults]);
                }

                var result = await processor.Process(node);
                processResults.Add(result);
            }

            LogExecutionCompleted(snapshot.Name);

            var successful = processResults.All(x => x.Successful);
            return (successful, [..processResults]);
        }
        catch (Exception e)
        {
            LogUnexpectedErrorDuringRun($"{snapshot.Id}_{snapshot.Name}", e.Message);

            processResults.Add(ProcessingResult.Error(
                $"Unexpected error during workflow {snapshot.Id}_{snapshot.Name} run: {e.Message}", string.Empty));

            return (false, [..processResults]);
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

    [LoggerMessage(LogLevel.Error, "Unexpected error during workflow {workflow} run: {errorMessage}")]
    partial void LogUnexpectedErrorDuringRun(string workflow, string errorMessage);

    [LoggerMessage(LogLevel.Information, "Workflow '{workflowName}' completed.")]
    partial void LogExecutionCompleted(string workflowName);

}
