using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared;
using WfAssist.Workflows.Api.Features;

namespace WfAssist.Workflows.Api;

internal static class Endpoints
{
    public static void MapWorkflowEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var workflowsGroup = endpointBuilder
            .MapGroup("/workflows")
            .WithTags($"{Constants.AppName}_Workflows");

        workflowsGroup.MapGetWorkflowIdentitiesEndpoint();
        workflowsGroup.MapGetWorkflowByIdEndpoint();

        workflowsGroup.MapCreateWorkflowEndpoint();
        workflowsGroup.MapRenameEndpoint();
        workflowsGroup.MapUpdateDataEndpoint();
        workflowsGroup.MapDeleteWorkflowEndpoint();

        workflowsGroup.MapSubscribeToWorkflowEventsEndpoint();
        workflowsGroup.MapQueueWorkflowExecutionEndpoint();
    }
}