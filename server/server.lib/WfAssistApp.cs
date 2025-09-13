using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Infrastructure;
using WfAssist.AspNetCore.Shared;

namespace WfAssist.AspNetCore;

public static class WfAssistApp
{
    private static readonly FeatureModuleManager FeatureModuleManager = new();

    /// <summary>
    /// Adds services required by WfAssist app.
    /// </summary>
    public static void AddWfAssistServices(this IServiceCollection services)
    {
        var wfAssistAssembly = typeof(WfAssistApp).Assembly;
        services.AddSingleton<IReadOnlyDbConnectionFactory, SqliteReadOnlyDbConnectionFactory>();
        services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>();

        services.AddScoped<IReadOnlyUnitOfWork, ReadOnlyUnitOfWork>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(Constants.SqliteDbConnectionString)
                .ScanIn(wfAssistAssembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        FeatureModuleManager.RegisterModules(services, wfAssistAssembly);
    }

    /// <summary>
    /// Sets up WfAssist resources and api.<br/><br/>
    /// <b>Client endpoints:</b><br/>
    /// <inheritdoc cref="WfAssistClientEndpoints.RegisterWfAssistClientEndpoints"/><br/><br/>
    /// <b>Feature modules (api):</b><br/>
    /// 1. <inheritdoc cref="FeatureModuleManager.InitializeModules"/><br/>
    /// 2. <inheritdoc cref="FeatureModuleManager.MapFeatureModulesEndpoints"/><br/>
    /// </summary>
    /// <param name="app">Web application that want to use WfAssist.</param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static async Task UseWfAssistApp(this WebApplication app, bool excludeFromOpenApi = true)
    {
        UpdateDatabase(app);

        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(UseWfAssistApp)}-API_and_UI_registration");

        var wfAssistDefaultRouteGroup = app.MapGroup(Constants.AppRoute);
        if (excludeFromOpenApi)
        {
            wfAssistDefaultRouteGroup.ExcludeFromDescription();
        }

        await FeatureModuleManager.InitializeModules(app);
        FeatureModuleManager.MapFeatureModulesEndpoints(wfAssistDefaultRouteGroup);

        WfAssistClientEndpoints.RegisterWfAssistClientEndpoints(wfAssistDefaultRouteGroup, logger);
    }

    private static void UpdateDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}