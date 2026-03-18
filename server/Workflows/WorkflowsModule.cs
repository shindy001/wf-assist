using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using WfAssist.Workflows.Api.Workflows;
using WfAssist.Workflows.Core.Runtime;
using WfAssist.Workflows.Core.Runtime.NodeProcessors;
using WfAssist.Workflows.Core.Services;
using WfAssist.Workflows.Infrastructure;
using WfAssist.Workflows.Infrastructure.Serialization;

namespace WfAssist.Workflows;

public static class WorkflowsModule
{
    public static void AddWorkflows(this IServiceCollection services)
    {
        var wfAssistAssembly = typeof(WorkflowsModule).Assembly;

        services.AddFluentMigratorCore()
            .ConfigureRunner(cfg => cfg
                .AddSQLite()
                .WithGlobalConnectionString(Constants.SqliteDbConnectionString)
                .ScanIn(wfAssistAssembly).For.Migrations())
            .AddLogging(cfg => cfg.AddFluentMigratorConsole());

        // Dapper types customization
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new WorkflowSnapshotTypeHandler());
        SqlMapper.AddTypeHandler(new WorkflowDataTypeHandler());
        SqlMapper.AddTypeHandler(new ProcessingResultDictionaryTypeHandler());

        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IExecutionRepository, ExecutionRepository>();

        services.AddHttpClient();
        services.AddScoped<ProcessingContext>();
        services.AddScoped<WorkflowNodeReferenceResolver>();

        services.AddScoped<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>();
        services.AddScoped<IWorkflowNodeProcessor, HeadersWorkflowNodeProcessor>();
        services.AddScoped<WorkflowNodeProcessorProvider>();

        services.AddScoped<WorkflowExecutor>();
        services.AddScoped<ExecutionManager>();

        services.AddSingleton<INotificationDispatcher, NotificationDispatcher>();
        services.AddHostedService<ExecutionBackgroundService>();
    }

    public static void MapWorkflows(this WebApplication app)
    {
        UpdateDatabase(app);

        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(WorkflowsModule)}-API_and_UI_registration");

        var wfAssistApiDefaultRouteGroup = app
            .MapGroup(Constants.ApiRoute)
            .WithTags(Constants.ApiRoute)
            // TODO - remove when auth is supported
            .AllowAnonymous();

        // Api endpoints
        wfAssistApiDefaultRouteGroup.MapWorkflowEndpoints();
    }

    private static void UpdateDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}