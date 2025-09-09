using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WfAssist.AspNetCore;

public static class WfAssistApp
{
    /// <summary>
    /// Sets up WfAssist resources and api.<br/><br/>
    /// <b>Client endpoints:</b><br/>
    /// <inheritdoc cref="WfAssistClientEndpoints.RegisterWfAssistClientEndpoints"/><br/><br/>
    /// <b>Api endpoints:</b><br/>
    /// <inheritdoc cref="WfAssistApiEndpoints.RegisterWfAssistApiEndpoints"/><br/>
    /// </summary>
    /// <param name="endpoints">Endpoints of the application that want to use WfAssist.</param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static void UseWfAssistApp(this IEndpointRouteBuilder endpoints, bool excludeFromOpenApi = true)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(UseWfAssistApp)}-API_and_UI_registration");

        endpoints.RegisterWfAssistClientEndpoints(logger, excludeFromOpenApi);
        endpoints.RegisterWfAssistApiEndpoints(logger, excludeFromOpenApi);
    }
}