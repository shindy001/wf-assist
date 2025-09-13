using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.AspNetCore.Shared;

public interface IFeatureModule
{
    /// <summary>
    /// Registers service dependencies required by the module.
    /// </summary>
    void RegisterServices(IServiceCollection services);

    /// <summary>
    /// Initializes feature. E.g. Seeding of default data.
    /// </summary>
    Task Initialize(AsyncServiceScope serviceScope);

    /// <summary>
    /// Maps endpoints required by the module.
    /// </summary>
    void MapEndpoints(IEndpointRouteBuilder endpointBuilder);
}