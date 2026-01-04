using Microsoft.Extensions.DependencyInjection;
using WfAssist.AspNetCore.Api.Workflows;
using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Core.Runtime.NodeProcessors;

internal sealed class HeadersWorkflowNodeProcessor : IWorkflowNodeProcessor
{
    private readonly HttpClient _httpClient;
    private readonly WorkflowNodeReferenceResolver _nodeReferenceResolver;

    public HeadersWorkflowNodeProcessor(
        [FromKeyedServices(WorkflowConstants.HttpClientServiceKey)] HttpClient httpClient,
        WorkflowNodeReferenceResolver nodeReferenceResolver)
    {
        _httpClient = httpClient;
        _nodeReferenceResolver = nodeReferenceResolver;
    }

    public Task<ProcessingResult> Process(WorkflowNode workflowNode)
    {
        if (workflowNode.Data is not HeadersNodeData headersNodeData)
        {
            throw new ArgumentException($"Expected node data type {nameof(HeadersNodeData)} but got {workflowNode.Data.GetType().Name}");
        }

        var resolvedData = ResolveNodeReferences(headersNodeData);
        foreach (var header in resolvedData.Headers)
        {
            // TryAddWithoutValidation does not throw when invalid header like Content-Type(http response specific) is added to default headers
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);
        }

        return Task.FromResult(ProcessingResult.Success(ProcessResultValueType.None));
    }

    private HeadersNodeData ResolveNodeReferences(HeadersNodeData headersNodeData)
    {
        return headersNodeData with
        {
            Headers = headersNodeData.Headers
                .Select(header => header with {Value = _nodeReferenceResolver.Resolve(header.Value)}).ToList()
        };
    }
}
