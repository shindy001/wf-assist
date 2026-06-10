using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WfAssist.Executions.Api;
using WfAssist.Executions.Contracts;
using WfAssist.Executions.Infrastructure;
using WfAssist.Shared;

namespace WfAssist.Executions;

public static class ExecutionsModule
{
    public static void AddExecutions(this IServiceCollection services)
    {
        services.AddDbContext<ExecutionsDbContext>((sp, options) =>
        {
            var connectionStringProvider = sp.GetRequiredService<IDbConnectionStringProvider>();
            var connectionString = connectionStringProvider.GetConnectionString("executions");
            options.UseSqlite(connectionString);
        });

        services.AddScoped<IExecutionsFacade, ExecutionsFacade>();
    }

    public static void MapExecutions(this WebApplication app)
    {
        UpdateDatabase(app);

        var wfAssistApiDefaultRouteGroup = app
            .MapGroup(Constants.ApiRoute)
            .WithTags(Constants.ApiRoute)
            // TODO - remove when auth is supported
            .AllowAnonymous();

        // Api endpoints
        wfAssistApiDefaultRouteGroup.MapExecutionEndpoints();
    }

    private static void UpdateDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExecutionsDbContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }
}