using Microsoft.AspNetCore.Builder;
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
    /// <param name="app">Web application that want to use WfAssist.</param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static void UseWfAssistApp(this WebApplication app, bool excludeFromOpenApi = true)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(UseWfAssistApp)}-API_and_UI_registration");

        app.RegisterWfAssistClientEndpoints(logger, excludeFromOpenApi);
        app.RegisterWfAssistApiEndpoints(logger, excludeFromOpenApi);
    }
}