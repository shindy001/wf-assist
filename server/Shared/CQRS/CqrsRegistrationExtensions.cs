using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.CQRS;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core cqrs services like <see cref="ICommandDispatcher"/>, <see cref="IQueryDispatcher"/>
    /// and other common services.
    /// </summary>
    public static void AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddCqrsServices();
    }

    /// <summary>
    /// Registers cqrs specific services from given assemblies like <see cref="ICommandHandler{TCommand,TResult}"/>
    /// and <see cref="IQueryHandler{TQuery,TResult}"/>.
    /// </summary>
    private static void AddCqrsServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}