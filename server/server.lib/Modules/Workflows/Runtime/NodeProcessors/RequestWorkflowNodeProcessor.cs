using Microsoft.Extensions.DependencyInjection;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

public sealed class RequestWorkflowNodeProcessor : IWorkflowNodeProcessor
{
    public const string HttpClientKey = "RequestProcessorHttpClient";

    private readonly HttpClient _httpClient;

    public RequestWorkflowNodeProcessor([FromKeyedServices(HttpClientKey)] HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ProcessResult> Process(WorkflowNode workflowNode)
    {
        if (workflowNode.Data is not RequestNodeData requestNodeData)
        {
            throw new ArgumentException($"Expected node data type {nameof(RequestNodeData)} but got {workflowNode.Data.GetType()}");
        }

        // TODO
        // 1. Make a request
        // 2. Convert result to JsonDocument (maybe custom model like ResponseResult with simplified data from response - Status, data, error???)
        // 3. Return ProcessResult.Success/Error according to response
        throw new NotImplementedException();
    }
}

public static class RequestNodeProcessorExtensions
{
    public static void RegisterRequestNodeKeyedProcessor(this IServiceCollection services)
    {
        services.AddHttpClient(RequestWorkflowNodeProcessor.HttpClientKey).AddAsKeyed(ServiceLifetime.Transient);
        services.AddKeyedSingleton<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>(nameof(RequestNodeData));
    }
}