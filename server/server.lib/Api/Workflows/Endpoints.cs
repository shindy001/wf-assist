using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Api.Workflows.Features;

namespace WfAssist.AspNetCore.Api.Workflows;

internal static class Endpoints
{
    public static void MapWorkflowEndpoints(this IEndpointRouteBuilder endpointBuilder)
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

        workflowsGroup.MapSubscribeEndpoint();
        workflowsGroup.MapQueueRunEndpoint();
    }
}