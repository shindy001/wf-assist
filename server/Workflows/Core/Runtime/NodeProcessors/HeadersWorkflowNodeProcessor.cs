using WfAssist.Shared.Contracts;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Core.Runtime.NodeProcessors;

internal sealed class HeadersWorkflowNodeProcessor : IWorkflowNodeProcessor
{
    private readonly HttpClient _httpClient;
    private readonly WorkflowNodeReferenceResolver _nodeReferenceResolver;

    public HeadersWorkflowNodeProcessor(
        HttpClient httpClient,
        WorkflowNodeReferenceResolver nodeReferenceResolver)
    {
        _httpClient = httpClient;
        _nodeReferenceResolver = nodeReferenceResolver;
    }

    public Task<ProcessingResult> Process(Node node)
    {
        if (node is not HeadersNode headersNode)
        {
            throw new ArgumentException($"Expected node of type {nameof(HeadersNode)} but got {node.GetType().Name}");
        }

        var resolvedData = ResolveNodeReferences(headersNode);
        foreach (var header in resolvedData.Headers)
        {
            // TryAddWithoutValidation does not throw when invalid header like Content-Type(http response specific) is added to default headers
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);
        }

        return Task.FromResult(ProcessingResult.Success(ProcessResultValueType.None));
    }

    private HeadersNode ResolveNodeReferences(HeadersNode headersNode)
    {
        return headersNode with
        {
            Headers = headersNode.Headers
                .Select(header => header with {Value = _nodeReferenceResolver.Resolve(header.Value)}).ToList()
        };
    }
}
