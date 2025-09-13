using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WfAssist.AspNetCore.Shared;

namespace WfAssist.AspNetCore.Features.Workflows;

internal sealed class WorkflowsFeature : IFeatureModule
{
    public void RegisterServices(IServiceCollection services)
    {
    }

    public Task Initialize(AsyncServiceScope serviceScope)
    {
        return Task.CompletedTask;
    }

    public void MapEndpoints(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapGet("/workflows", () => "Hello from workflows feature!");
    }
}