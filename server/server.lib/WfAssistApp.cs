using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace server.lib;

public static class WfAssistApp
{
    /// <summary>
    /// 1. Registers static file hosting for wwwroot folder<br/>
    /// 2. Registers api endpoints used by WfAssist app<br/>
    /// 3. WfAssist client app and this lib project is bundled to a nuget in nuget project (on project build) in solution.
    /// Nuget package output dir is [solutionDir/nuget/packages]<br/>
    /// 4. If you are not using the nuget, you need to copy dist binaries to [your server host outputDir]/wwwroot/wfAssist
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

        // Redirect to client index - wfAssist files (from nuget or dist) must be in [server host outputDir]/wwwroot/wfAssist
        app.MapGet("/wfAssist", (context) =>
        {
            context.Response.Redirect("/wfAssist/index.html");
            return Task.CompletedTask;
        });

        // TODO - api endpoints
        app.MapGet("/api", () => "Hello World!");

    }
}