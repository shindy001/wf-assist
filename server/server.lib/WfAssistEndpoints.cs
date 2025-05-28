using Microsoft.AspNetCore.Builder;

namespace server.lib;

public static class WfAssistEndpoints
{
    public static void MapWfAssistEndpoints(this WebApplication app)
    {
        // TODO - Add endpoints
        app.MapGet("/", () => "Hello World!");
    }
}