using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using OneOf;
using OneOf.Types;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

internal sealed class RequestWorkflowNodeProcessor : IWorkflowNodeProcessor
{
    public const string HttpClientKey = "RequestProcessorHttpClient";

    private readonly HttpClient _httpClient;
    private readonly ProcessingContext _processingContext;
    private readonly WorkflowNodeReferenceResolver _nodeReferenceResolver;

    public RequestWorkflowNodeProcessor([FromKeyedServices(HttpClientKey)] HttpClient httpClient,
        ProcessingContext processingContext,
        WorkflowNodeReferenceResolver nodeReferenceResolver)
    {
        _httpClient = httpClient;
        _processingContext = processingContext;
        _nodeReferenceResolver = nodeReferenceResolver;
    }

    public async Task<OneOf<Success, Error>> Process(WorkflowNode workflowNode)
    {
        if (workflowNode.Data is not RequestNodeData requestNodeData)
        {
            throw new ArgumentException($"Expected node data type {nameof(RequestNodeData)} but got {workflowNode.Data.GetType()}");
        }

        var data = ResolveNodeReferences(requestNodeData);
        var requestMessage = GetRequestMessage(data.RequestType, data.Url, data.RequestBody);

        var response = await _httpClient.SendAsync(requestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _processingContext.AddResult(workflowNode.Id, ProcessingResult.Error(
                $"Status code: {response.StatusCode}, reason: {response.ReasonPhrase}",
                responseBody));
            return new Error();
        }

        if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Json)
        {
            _processingContext.AddResult(workflowNode.Id, ProcessingResult.Error(
                $"Unsupported Response content MediaType '{response.Content.Headers.ContentType?.MediaType}'. " +
                $"Only '{MediaTypeNames.Application.Json}' is supported.",
                workflowNode.Id));
            return new Error();
        }

        var (resultValueType, resultData) = ParseResponse(responseBody.Trim());
        _processingContext.AddResult(workflowNode.Id,
            ProcessingResult.Success(workflowNode.Id, resultValueType, resultData));
        return new Success();
    }

    private RequestNodeData ResolveNodeReferences(RequestNodeData requestNodeData)
    {
        return requestNodeData with
        {
            Url = _nodeReferenceResolver.Resolve(requestNodeData.Url),
            RequestBody = _nodeReferenceResolver.Resolve(requestNodeData.RequestBody ?? string.Empty)
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

public static class RequestNodeProcessorExtensions
{
    public static void RegisterRequestNodeKeyedProcessor(this IServiceCollection services)
    {
        services.AddHttpClient(RequestWorkflowNodeProcessor.HttpClientKey).AddAsKeyed(ServiceLifetime.Transient);
        services.AddKeyedScoped<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>(nameof(RequestNodeData));
    }
}