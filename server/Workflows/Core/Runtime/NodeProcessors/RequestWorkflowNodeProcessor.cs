using System.Net.Mime;
using System.Text;
using System.Text.Json;
using WfAssist.Shared.Contracts;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Core.Runtime.NodeProcessors;

internal sealed class RequestWorkflowNodeProcessor : IWorkflowNodeProcessor
{
    private readonly HttpClient _httpClient;
    private readonly WorkflowNodeReferenceResolver _nodeReferenceResolver;

    public RequestWorkflowNodeProcessor(
        HttpClient httpClient,
        WorkflowNodeReferenceResolver nodeReferenceResolver)
    {
        _httpClient = httpClient;
        _nodeReferenceResolver = nodeReferenceResolver;
    }

    public async Task<ProcessingResult> Process(WorkflowNode workflowNode)
    {
        if (workflowNode.Data is not RequestNodeData requestNodeData)
        {
            throw new ArgumentException($"Expected node data type {nameof(RequestNodeData)} but got {workflowNode.Data.GetType().Name}");
        }

        var data = ResolveNodeReferences(requestNodeData);
        var requestMessage = GetRequestMessage(data.RequestType, data.Url, data.RequestBody);

        var response = await _httpClient.SendAsync(requestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return ProcessingResult.Error(
                $"Status code: {response.StatusCode}, reason: {response.ReasonPhrase}",
                responseBody);
        }

        if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Json)
        {
            return ProcessingResult.Error(
                $"Unsupported Response content MediaType '{response.Content.Headers.ContentType?.MediaType}'. " +
                $"Only '{MediaTypeNames.Application.Json}' is supported.");
        }

        var (resultValueType, resultData) = ParseResponse(responseBody.Trim());

        return ProcessingResult.Success(resultValueType, resultData);
    }

    private RequestNodeData ResolveNodeReferences(RequestNodeData requestNodeData)
    {
        return requestNodeData with
        {
            Url = _nodeReferenceResolver.Resolve(requestNodeData.Url),
            RequestBody = requestNodeData.RequestBody is null
                ? null
                : _nodeReferenceResolver.Resolve(requestNodeData.RequestBody)
        };
    }

    private static (ProcessResultValueType resultValueType, object? resultData) ParseResponse(string responseBody)
    {
        if (responseBody == string.Empty)
        {
            return (ProcessResultValueType.None, null);
        }

        var document = JsonDocument.Parse(responseBody);
        return (ProcessResultValueType.JsonDocument, document);
    }

    private static HttpRequestMessage GetRequestMessage(RequestType requestType, string url, string? body)
    {
        var content = new StringContent(body ?? string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json);
        return requestType switch {
            RequestType.Get => new HttpRequestMessage(HttpMethod.Get, url),
            RequestType.Post => new HttpRequestMessage(HttpMethod.Post, url) { Content = content },
            RequestType.Put => new HttpRequestMessage(HttpMethod.Put, url) { Content = content },
            RequestType.Patch => new HttpRequestMessage(HttpMethod.Patch, url) { Content = content },
            RequestType.Delete => new HttpRequestMessage(HttpMethod.Delete, url) { Content = content },
            _ => throw new InvalidOperationException($"Unsupported request type {requestType}")
        };
    }
}
