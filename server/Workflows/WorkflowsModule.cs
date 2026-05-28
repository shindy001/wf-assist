using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WfAssist.Shared;
using WfAssist.Shared.CQRS;
using WfAssist.Shared.Notifications;
using WfAssist.Workflows.Api;
using WfAssist.Workflows.Core.Runtime;
using WfAssist.Workflows.Core.Runtime.NodeProcessors;
using WfAssist.Workflows.Infrastructure;

namespace WfAssist.Workflows;

public static class WorkflowsModule
{
    public static void AddWorkflows(this IServiceCollection services)
    {
        // Register command/query handlers from WorkflowModule
        var assembly = typeof(WorkflowsModule).Assembly;
        services.AddCqrsServices(serviceAssemblies: assembly);
        // Register notification types used by INotificationDispatcher
        services.RegisterNotificationTypes(notificationAssemblies: assembly);

        services.AddDbContext<WorkflowsDbContext>((sp, options) =>
        {
            var connectionStringProvider = sp.GetRequiredService<IDbConnectionStringProvider>();
            var connectionString = connectionStringProvider.GetConnectionString("workflows");
            options.UseSqlite(connectionString);
        });

        services.AddScoped<IUnitOfWork, WorkflowsDbContext>();
        services.AddScoped<ProcessingContext>();
        services.AddScoped<WorkflowNodeReferenceResolver>();

        services.AddScoped<IWorkflowNodeProcessor, RequestWorkflowNodeProcessor>();
        services.AddScoped<IWorkflowNodeProcessor, HeadersWorkflowNodeProcessor>();
        services.AddScoped<WorkflowNodeProcessorProvider>();

        services.AddScoped<WorkflowExecutor>();
        services.AddScoped<ExecutionManager>();

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
        var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowsDbContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }
}