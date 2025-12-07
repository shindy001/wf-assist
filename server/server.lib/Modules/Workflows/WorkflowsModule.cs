using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WfAssist.AspNetCore.Core;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Features;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure.Serialization;
using WfAssist.AspNetCore.Modules.Workflows.Runtime;
using WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

namespace WfAssist.AspNetCore.Modules.Workflows;

internal static class WorkflowsModule
{
    public static void AddWorkflowsModuleServices(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new WorkflowSnapshotTypeHandler());
        SqlMapper.AddTypeHandler(new WorkflowDataTypeHandler());
        SqlMapper.AddTypeHandler(new ProcessingResultDictionaryTypeHandler());

        services.AddScoped<WorkflowRepository>();
        services.AddScoped<WorkflowProcessingRepository>();

        services.AddScoped<ProcessingContext>();
        services.AddScoped<WorkflowNodeReferenceResolver>();
        services.AddHttpClient(WorkflowConstants.HttpClientServiceKey).AddAsKeyed();
        services.AddKeyedScoped<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>(WorkflowConstants.RequestNodeProcessorKey);
        services.AddKeyedScoped<IWorkflowNodeProcessor, HeadersWorkflowNodeProcessor>(WorkflowConstants.HeadersNodeProcessorKey);
        services.AddScoped<WorkflowExecutor>();

        services.AddSingleton<NotificationDispatcher>();
        services.AddHostedService<WorkflowRunnerBackgroundService>();
    }

    public static Task InitializeWorkflowsModule(this IServiceProvider serviceProvider)
    {
        // TODO - Maybe seed some data if DB is empty???
        return Task.CompletedTask;
    }

    public static void MapWorkflowsModuleEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var workflowsGroup = endpointBuilder
            .MapGroup("/workflows")
            .WithTags($"{Constants.AppName}_Workflows");

        workflowsGroup.MapGetIdentitiesEndpoint();
        workflowsGroup.MapGetByIdEndpoint();

        workflowsGroup.MapCreateEndpoint();
        workflowsGroup.MapRenameEndpoint();
        workflowsGroup.MapUpdateDataEndpoint();
        workflowsGroup.MapDeleteEndpoint();

        workflowsGroup.MapSubscribeEndpoint();
        workflowsGroup.MapQueueRunEndpoint();
    }
}