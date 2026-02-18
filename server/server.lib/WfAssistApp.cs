using System.Text.Json.Serialization;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using WfAssist.AspNetCore.Api;
using WfAssist.AspNetCore.Api.Workflows;
using WfAssist.AspNetCore.Core.Runtime;
using WfAssist.AspNetCore.Core.Runtime.NodeProcessors;
using WfAssist.AspNetCore.Core.Services;
using WfAssist.AspNetCore.Infrastructure;
using WfAssist.AspNetCore.Infrastructure.Middleware;
using WfAssist.AspNetCore.Infrastructure.Serialization;

namespace WfAssist.AspNetCore;


public static class WfAssistApp
{
    private const string CorsAllowAllPolicy = "AllowAllCorsPolicy";

    /// <summary>
    /// Registers WfAssist app as separate process.
    /// </summary>
    public static void RegisterWfAssistApp(this WebApplication hostApp, int httpPort = 7130)
    {
        var builder = WebApplication.CreateBuilder();
        builder.ConfigureWfAssistAppBuilder();

        var wfAssistApp = builder.Build();
        wfAssistApp.ConfigureWfAssistApp();

        // Careful - Debug mode in IDEs does not trigger these callbacks, but this host is still killed as child process
        // If this become a problem, wrap the wfAssist builder to IHostedService as Dashboard app in Aspire (DashboardServiceHost.cs) do it.
        hostApp.Lifetime.ApplicationStarted.Register(() =>
            {
                var url = $"http://localhost:{httpPort}";
                Log.Logger.Information("Starting WfAssist api server - {Url}/scalar.", url);
                Log.Logger.Information("Starting WfAssist app - {Url}/wfAssist.", url);
                wfAssistApp.Run(url);
            }
        );

        hostApp.Lifetime.ApplicationStopping.Register(() =>
            {
                Log.Logger.Information("Stopping WfAssist api and client app.");
                wfAssistApp.StopAsync().GetAwaiter().GetResult();
            }
        );
    }

    internal static void ConfigureWfAssistAppBuilder(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        builder.Services.AddSerilog();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            // Force openapi to use strict number handling instead of default AllowReadingFromString
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        });

        builder.Services.AddOpenApi(opt =>
        {
            opt.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "WFAssist API Reference",
                    Description = "API for WFAssist client."
                };
                return Task.CompletedTask;
            });
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsAllowAllPolicy, policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        });

        var wfAssistAssembly = typeof(WfAssistApp).Assembly;

        builder.Services.AddFluentMigratorCore()
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

        builder.Services.AddScoped<IDbConnectionProvider, SqliteDbConnectionProvider>();
        builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        builder.Services.AddScoped<IExecutionRepository, ExecutionRepository>();

        builder.Services.AddHttpClient();
        builder.Services.AddScoped<ProcessingContext>();
        builder.Services.AddScoped<WorkflowNodeReferenceResolver>();
        builder.Services.AddKeyedScoped<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>(ProcessorConstants.RequestNodeProcessorKey);
        builder.Services.AddKeyedScoped<IWorkflowNodeProcessor, HeadersWorkflowNodeProcessor>(ProcessorConstants.HeadersNodeProcessorKey);
        builder.Services.AddScoped<WorkflowExecutor>();
        builder.Services.AddScoped<ExecutionManager>();

        builder.Services.AddSingleton<INotificationDispatcher, NotificationDispatcher>();
        builder.Services.AddHostedService<ExecutionBackgroundService>();
    }

    internal static void ConfigureWfAssistApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(CorsAllowAllPolicy);
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
                options.EnabledClients = [ScalarClient.Curl, ScalarClient.Axios];
                options.EnabledTargets = [ScalarTarget.Shell, ScalarTarget.JavaScript];

            });
        }

        app.UseWfAssistAppClient();
        app.UseWfAssistAppServer();
    }

    private static void UseWfAssistAppClient(this WebApplication app)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(RegisterWfAssistApp)}-Client_and_UI_registration");

        var wfAssistClientDefaultRouteGroup = app
            .MapGroup(Constants.AppRoute)
            .WithTags(Constants.AppName)
            // TODO - remove when auth is supported
            .AllowAnonymous();

        wfAssistClientDefaultRouteGroup.MapWfAssistClientEndpoints(logger);
    }

    private static void UseWfAssistAppServer(this WebApplication app)
    {
        UpdateDatabase(app);

        app.UseTransactionMiddleware();

        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(RegisterWfAssistApp)}-API_and_UI_registration");

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