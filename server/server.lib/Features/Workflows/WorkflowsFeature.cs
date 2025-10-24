using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WfAssist.AspNetCore.Shared;

namespace WfAssist.AspNetCore.Features.Workflows;

[UsedImplicitly]
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
        var workflowsGroup = endpointBuilder
            .MapGroup("/workflows")
            .WithTags($"{Constants.AppName}_Workflows");

        workflowsGroup.MapGetIdentitiesEndpoint();
        workflowsGroup.MapGetByIdEndpoint();

        workflowsGroup.MapCreateEndpoint();
        workflowsGroup.MapRenameEndpoint();
        workflowsGroup.MapUpdateDataEndpoint();
        workflowsGroup.MapDeleteEndpoint();
    }
}