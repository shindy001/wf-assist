using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.CQRS;

public static class ServiceCollectionExtensions
{
    public static void AddCqrsServices(this IServiceCollection services, params Assembly[] cqrsHandlerAssemblies)
    {
        services.Scan(scan => scan
            .FromAssemblies(cqrsHandlerAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
    }
}