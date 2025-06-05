using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace server.lib;

public static class WfAssistApp
{
    /// <summary>
    /// 1. Registers static file hosting for wwwroot folder
    /// 2. Registers api endpoints used by WfAssist app
    /// 3. WfAssist app is available at [yourAppUrl]/wfAssist
    /// </summary>
    /// <param name="app"></param>
    public static void UseWfAssistApp(this WebApplication app)
    {
        // Register wwwroot static file hosting
        var staticFilesPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(staticFilesPath)
        });

        // Redirect to index
        app.MapGet("/wfAssist", (context) =>
        {
            context.Response.Redirect("/wfAssist/index.html");
            return Task.CompletedTask;
        });

        // TODO - api endpoints
        app.MapGet("/api", () => "Hello World!");

    }
}