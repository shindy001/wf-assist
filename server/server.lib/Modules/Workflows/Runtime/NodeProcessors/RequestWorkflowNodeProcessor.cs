using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

internal sealed class RequestWorkflowNodeProcessor : IWorkflowNodeProcessor
{
    public const string HttpClientKey = "RequestProcessorHttpClient";

    private readonly HttpClient _httpClient;
    private readonly ProcessingContext _processingContext;

    public RequestWorkflowNodeProcessor([FromKeyedServices(HttpClientKey)] HttpClient httpClient,
        ProcessingContext processingContext)
    {
        _httpClient = httpClient;
        _processingContext = processingContext;
    }

    public async Task<OneOf<Success, Error>> Process(WorkflowNode workflowNode)
    {
        if (workflowNode.Data is not RequestNodeData requestNodeData)
        {
            throw new ArgumentException($"Expected node data type {nameof(RequestNodeData)} but got {workflowNode.Data.GetType()}");
        }

        var requestMessage = GetRequestMessage(requestNodeData.RequestType, requestNodeData.Url, requestNodeData.RequestBody);
        var response = await _httpClient.SendAsync(requestMessage);

        if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Json)
        {
            _processingContext.AddResult(workflowNode.Id, ProcessingResult.Error(
                $"Unsupported Response content MediaType '{response.Content.Headers.ContentType?.MediaType}'. " +
                $"Only '{MediaTypeNames.Application.Json}' is supported.",
                workflowNode.Id));
            return new Error();
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _processingContext.AddResult(workflowNode.Id, ProcessingResult.Error(
                $"Status code: {response.StatusCode}, reason: {response.ReasonPhrase}",
                responseBody));
            return new Error();
        }

        var (resultValueType, resultData) = ParseResponse(responseBody.Trim());
        _processingContext.AddResult(workflowNode.Id,
            ProcessingResult.Success(workflowNode.Id, resultValueType, resultData));
        return new Success();
    }

    private static (ProcessResultValueType resultValueType, object? resultData) ParseResponse(string responseBody)
    {
        if (responseBody == string.Empty)
        {
            return (ProcessResultValueType.None, null);
        }

        return IsValidJsonDocument(responseBody, out var document)
            ? (ProcessResultValueType.JsonDocument, document)
            : (ProcessResultValueType.String, responseBody);
    }

    private static HttpRequestMessage GetRequestMessage(RequestType requestType, string url, string? body)
    {
        return requestType switch {
            RequestType.Get => new HttpRequestMessage(HttpMethod.Get, url),
            RequestType.Post => new HttpRequestMessage(HttpMethod.Get, url) { Content = new StringContent(body ?? string.Empty) },
            RequestType.Put => new HttpRequestMessage(HttpMethod.Get, url) { Content = new StringContent(body ?? string.Empty) },
            RequestType.Patch => new HttpRequestMessage(HttpMethod.Get, url) { Content = new StringContent(body ?? string.Empty) },
            RequestType.Delete => new HttpRequestMessage(HttpMethod.Get, url) { Content = new StringContent(body ?? string.Empty) },
            _ => throw new InvalidOperationException($"Unsupported request type {requestType}")
        };
    }

    private static bool IsValidJsonDocument(string jsonString, out JsonDocument? document)
    {
        try
        {
            document = JsonDocument.Parse(jsonString);
            return true;
        }
        catch (JsonException)
        {
            document = null;
            return false;
        }
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