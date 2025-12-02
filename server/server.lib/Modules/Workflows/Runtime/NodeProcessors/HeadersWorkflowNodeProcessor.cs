using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

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

    public Task<OneOf<Success, Error>> Process(WorkflowNode workflowNode)
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

        return Task.FromResult<OneOf<Success, Error>>(new Success());
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
