using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WfAssist.AspNetCore;

internal static class WfAssistApiEndpoints
{
    /// <summary>
    /// 1. Registers WfAssist api endpoints
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="logger"></param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static void RegisterWfAssistApiEndpoints(this IEndpointRouteBuilder endpoints, ILogger logger, bool excludeFromOpenApi = true)
    {
        var apiRouteGroup = endpoints.MapGroup(Constants.ApiRoute).WithTags(Constants.AppRoute);
        if (excludeFromOpenApi)
        {
            apiRouteGroup.ExcludeFromDescription();
        }

        // TODO add api routes
        apiRouteGroup.MapGet("/hello", () => "Hello from WfAssist api endpoint!");
    }
}