using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Core.Runtime.NodeProcessors;

internal sealed class WorkflowNodeProcessorProvider
{
    private readonly IDictionary<Type, IWorkflowNodeProcessor> _processors;

    public WorkflowNodeProcessorProvider(IEnumerable<IWorkflowNodeProcessor> processors)
    {
        _processors = processors.ToDictionary(x => x.GetType());
    }

    public IWorkflowNodeProcessor GetProcessor(Node node)
    {
        var processorType = GetProcessorType(node);

        if (!_processors.TryGetValue(processorType, out var processor))
        {
            throw new InvalidOperationException(
                $"Processor of type {processorType.Name} not found in registered node processors. Please ensure that the processor is correctly registered in DI container.");
        }

        return processor;
    }

    private static Type GetProcessorType(Node node)
    {
        return node switch
        {
            RequestNode => typeof(RequestWorkflowNodeProcessor),
            HeadersNode => typeof(HeadersWorkflowNodeProcessor),
            _ => throw new InvalidOperationException($"Unknown node type {node.GetType().Name}.")
        };
    }
}