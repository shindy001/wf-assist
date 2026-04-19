using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Shared;
using Shared.CQRS;
using WfAssist.Workflows;

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
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console()
            .CreateLogger();

        builder.Services.AddSerilog();
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict; // do not allow Quoted numbers (JSON strings for number properties)
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

        builder.Services.AddScoped<IDbConnectionProvider, SqliteDbConnectionProvider>();
        builder.Services.AddCqrsCore();

        // Api modules
        builder.Services.AddWorkflows();
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

        // JS client
        app.MapWfAssistAppClient();

        // Api modules
        app.MapWorkflows();
    }

    private static void MapWfAssistAppClient(this WebApplication app)
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
}