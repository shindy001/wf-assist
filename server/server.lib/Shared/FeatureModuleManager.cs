using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.AspNetCore.Shared;

public sealed class FeatureModuleManager
{
    private readonly List<IFeatureModule> _registeredModules = [];

    /// <summary>
    /// Registers modules that implement <see cref="IFeatureModule"/> contract.
    /// </summary>
    public void RegisterModules(IServiceCollection services, params Assembly[] moduleAssemblies)
    {
        foreach (var assembly in moduleAssemblies)
        {
            RegisterModulesInternal(services, assembly);
        }
    }

    /// <summary>
    /// Maps <see cref="IFeatureModule"/>s endpoints.
    /// </summary>
    public void MapFeatureModulesEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        foreach (var module in _registeredModules)
        {
            module.MapEndpoints(routeBuilder);
        }
    }

    /// <summary>
    /// Runs initialization of all <see cref="IFeatureModule"/>s.
    /// </summary>
    public async Task InitializeModules(IApplicationBuilder appBuilder)
    {
        foreach (var module in _registeredModules)
        {
            await using var scope = appBuilder.ApplicationServices.CreateAsyncScope();
            await module.Initialize(scope);
        }
    }

    private void RegisterModulesInternal(IServiceCollection services, Assembly assembly)
    {
        var modules = DiscoverModules(assembly);
        foreach (var module in modules)
        {
            module.RegisterServices(services);
            _registeredModules.Add(module);
        }
    }

    private static IEnumerable<IFeatureModule> DiscoverModules(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => t.IsClass && t.IsAssignableTo(typeof(IFeatureModule)))
            .Select(Activator.CreateInstance)
            .Cast<IFeatureModule>();
    }
}