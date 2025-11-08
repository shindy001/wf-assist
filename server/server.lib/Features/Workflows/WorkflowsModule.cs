using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.AspNetCore.Features.Workflows;

internal static class WorkflowsModule
{
    public static void AddWorkflowsModuleServices(this IServiceCollection services)
    {
    }

    public static Task InitializeModule(AsyncServiceScope serviceScope)
    {
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
    }
}