using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Api;
using WfAssist.AspNetCore.Api.Workflows;
using WfAssist.AspNetCore.Core.Runtime;
using WfAssist.AspNetCore.Core.Runtime.NodeProcessors;
using WfAssist.AspNetCore.Infrastructure;
using WfAssist.AspNetCore.Infrastructure.Middleware;
using WfAssist.AspNetCore.Infrastructure.Serialization;

namespace WfAssist.AspNetCore;

public static class WfAssistApp
{
    /// <summary>
    /// Adds services required by WfAssist app.
    /// </summary>
    public static void AddWfAssistServices(this IServiceCollection services)
    {
        var wfAssistAssembly = typeof(WfAssistApp).Assembly;

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

        // Services
        services.AddScoped<IDbConnectionProvider, SqliteDbConnectionProvider>();
        services.AddScoped<WorkflowRepository>();
        services.AddScoped<ExecutionRepository>();

        services.AddScoped<ProcessingContext>();
        services.AddScoped<WorkflowNodeReferenceResolver>();
        services.AddHttpClient(WorkflowConstants.HttpClientServiceKey).AddAsKeyed();
        services.AddKeyedScoped<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>(WorkflowConstants.RequestNodeProcessorKey);
        services.AddKeyedScoped<IWorkflowNodeProcessor, HeadersWorkflowNodeProcessor>(WorkflowConstants.HeadersNodeProcessorKey);
        services.AddScoped<WorkflowExecutor>();

        services.AddSingleton<NotificationDispatcher>();
        services.AddHostedService<ExecutionBackgroundService>();
    }

    /// <summary>
    /// Sets up WfAssist resources and api. Api and resources use AllowAnonymous by default (auth is unsupported)<br/><br/>
    /// <b>Client endpoints:</b><br/>
    /// <inheritdoc cref="ClientEndpoints.MapWfAssistClientEndpoints"/><br/><br/>
    /// </summary>
    /// <param name="app">Web application that want to use WfAssist.</param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static Task UseWfAssistApp(this WebApplication app, bool excludeFromOpenApi = true)
    {
        UpdateDatabase(app);

        app.UseTransactionMiddleware();

        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(UseWfAssistApp)}-API_and_UI_registration");

        var wfAssistDefaultRouteGroup = app
            .MapGroup(Constants.AppRoute)
            .WithTags(Constants.AppName)
            // TODO - remove when auth is supported
            .AllowAnonymous();

        if (excludeFromOpenApi)
        {
            wfAssistDefaultRouteGroup.ExcludeFromDescription();
        }

        wfAssistDefaultRouteGroup.MapWfAssistClientEndpoints(logger);

        // Modules endpoints
        wfAssistDefaultRouteGroup.MapWorkflowEndpoints();

        return Task.CompletedTask;
    }

    private static void UpdateDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}