using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WfAssist.Executions.Api.Features;
using WfAssist.Shared;

namespace WfAssist.Executions.Api;

internal static class Endpoints
{
    public static void MapExecutionEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var executionsGroup = endpointBuilder
            .MapGroup("/executions")
            .WithTags($"{Constants.AppName}_Executions");

        executionsGroup.MapGetWorkflowExecutionsEndpoint();
    }
}