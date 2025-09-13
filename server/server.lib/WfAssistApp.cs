using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.AspNetCore.Infrastructure;

namespace WfAssist.AspNetCore;

public static class WfAssistApp
{
    /// <summary>
    /// Adds services required by WfAssist app.
    /// </summary>
    /// <param name="services"></param>
    public static void AddWfAssistServices(this IServiceCollection services)
    {
        services.AddSingleton<IReadOnlyDbConnectionFactory, SqliteReadOnlyDbConnectionFactory>();
        services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>();

        services.AddScoped<IReadOnlyUnitOfWork, ReadOnlyUnitOfWork>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(Constants.SqliteDbConnectionString)
                .ScanIn(typeof(WfAssistApp).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }

    /// <summary>
    /// Sets up WfAssist resources and api.<br/><br/>
    /// <b>Client endpoints:</b><br/>
    /// <inheritdoc cref="WfAssistClientEndpoints.RegisterWfAssistClientEndpoints"/><br/><br/>
    /// <b>Api endpoints:</b><br/>
    /// <inheritdoc cref="WfAssistApiEndpoints.RegisterWfAssistApiEndpoints"/><br/>
    /// </summary>
    /// <param name="app">Web application that want to use WfAssist.</param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static void UseWfAssistApp(this WebApplication app, bool excludeFromOpenApi = true)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(UseWfAssistApp)}-API_and_UI_registration");

        app.RegisterWfAssistClientEndpoints(logger, excludeFromOpenApi);
        app.RegisterWfAssistApiEndpoints(logger, excludeFromOpenApi);

        UpdateDatabase(app);
    }

    private static void UpdateDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}