using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Core.Runtime.NodeProcessors;

internal sealed class WorkflowNodeProcessorProvider
{
    private readonly IDictionary<Type, IWorkflowNodeProcessor> _processors;

    public WorkflowNodeProcessorProvider(IEnumerable<IWorkflowNodeProcessor> processors)
    {
        _processors = processors.ToDictionary(x => x.GetType());
    }

    public IWorkflowNodeProcessor GetProcessor(WorkflowNodeData nodeData)
    {
        var processorType = GetProcessorType(nodeData);

        if (!_processors.TryGetValue(processorType, out var processor))
        {
            throw new InvalidOperationException(
                $"Processor of type {processorType.Name} not found in registered node data processors. Please ensure that the processor is correctly registered in DI container.");
        }

        return processor;
    }

    private static Type GetProcessorType(WorkflowNodeData nodeData)
    {
        return nodeData switch
        {
            RequestNodeData => typeof(RequestWorkflowNodeProcessor),
            HeadersNodeData => typeof(HeadersWorkflowNodeProcessor),
            _ => throw new InvalidOperationException($"Unknown node data type {nodeData.GetType().Name}.")
        };
    }
}