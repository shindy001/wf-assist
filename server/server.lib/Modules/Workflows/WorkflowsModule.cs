using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WfAssist.AspNetCore.Domain.Workflows.Contracts;
using WfAssist.AspNetCore.Modules.Workflows.Features;
using WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

namespace WfAssist.AspNetCore.Modules.Workflows;

internal static class WorkflowsModule
{
    public static void AddWorkflowsModuleServices(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new WorkflowDataTypeHandler());

        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
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
    }
}