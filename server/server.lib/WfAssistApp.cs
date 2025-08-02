using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace WfAssist.AspNetCore;

public static class WfAssistApp
{
    private const string AppRoute = "wfAssist";
    private const string ApiRoute = $"{AppRoute}/api";
    private const string WwwRootDirectory = "wwwroot";
    private const string IndexHtmlFile = "index.html";
    private const string IndexCssFile = "index.css";
    private const string IndexJsFile = "index.js";
    private const string FaviconFile = "favicon.ico";

    /// <summary>
    /// 1. Registers api endpoints used by WfAssist app<br/>
    /// 3. WfAssist client app and this lib project is bundled to a nuget in nuget project (on project build) in solution.
    /// Nuget package output dir is [solutionDir/nuget/packages]<br/>
    /// 4. If you are not using the nuget, you need to copy dist binaries to [your server host outputDir]/wwwroot/wfAssist
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="excludeFromOpenApi">Default is true, excludes WfAssist endpoints from OpenApi definitions</param>
    public static void UseWfAssistApp(this IEndpointRouteBuilder endpoints, bool excludeFromOpenApi = true)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger($"{nameof(UseWfAssistApp)}-API_and_UI_registration");
        var clientUiEndpointsGroup = endpoints.MapGroup(AppRoute);
        var clientApiGroup = endpoints.MapGroup(ApiRoute).WithTags(AppRoute);
        if (excludeFromOpenApi)
        {
            clientUiEndpointsGroup.ExcludeFromDescription();
            clientApiGroup.ExcludeFromDescription();
        }

        clientUiEndpointsGroup.MapWfAssistClient(logger);

        // Redirect to client index - wfAssist files (from nuget or dist) must be in [server host outputDir]/wwwroot/wfAssist
        endpoints.MapGet($"/{AppRoute}", context =>
        {
            context.Response.Redirect($"/{AppRoute}/{IndexHtmlFile}");
            return Task.CompletedTask;
        });

        // TODO add api routes
        clientApiGroup.MapGet("/hello", () => "Hello from WfAssist endpoint!");

    }

    /// <summary>
    /// Maps WfAssist client resources and provides them via endpoints, client resources are copies to output only when used via nuget.
    /// Nuget project bundles this server.lib and client binaries and copy them to output directory when the project using the nuget is build.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    private static IEndpointRouteBuilder MapWfAssistClient(this IEndpointRouteBuilder endpoints, ILogger logger)
    {
        var rootDirectoryPath = Path.Combine(AppContext.BaseDirectory, WwwRootDirectory, AppRoute);
        var rootDirectoryInfo = new DirectoryInfo(rootDirectoryPath);
        if (!rootDirectoryInfo.Exists)
        {
            var errorMessage =
                $"""
                  *****
                  Failed to register WFAssist UI, web application binaries not found in directory: '{rootDirectoryPath}'.
                  If your are developing or debugging WFAssist server and need the client UI, copy built client UI binaries to the specified directory.
                  Otherwise only server API will be available.
                  (You can ignore this message if you are only using the API or running WFAssist UI as separate process.)
                  *****
                  """;

            logger.LogError("{errorMessage}", errorMessage);

            return endpoints;
        }

        var indexHtmlFileInfo = rootDirectoryInfo.GetFiles(IndexHtmlFile).SingleOrDefault();
        var indexCssFileInfo = rootDirectoryInfo.GetFiles(IndexCssFile).SingleOrDefault();
        var indexJsFileInfo = rootDirectoryInfo.GetFiles(IndexJsFile).SingleOrDefault();
        var faviconFileInfo = rootDirectoryInfo.GetFiles(FaviconFile).SingleOrDefault();

        if (indexHtmlFileInfo is null || indexCssFileInfo is null || indexJsFileInfo is null || faviconFileInfo is null)
        {
            var errorMessage =
                $"""
                  *****
                  Failed to register WFAssist UI, some files are missing in app root directory '{rootDirectoryPath}':
                  {IndexHtmlFile}: {(indexHtmlFileInfo is null ? "missing": "found")}
                  {IndexCssFile}: {(indexCssFileInfo is null ? "missing": "found")}
                  {IndexJsFile}: {(indexJsFileInfo is null ? "missing": "found")}
                  {FaviconFile}: {(faviconFileInfo is null ? "missing": "found")}
                  *****
                  """;

            logger.LogError("{errorMessage}", errorMessage);

            return endpoints;
        }

        endpoints.MapDocumentEndpoint(indexHtmlFileInfo);
        endpoints.MapStaticAsset(indexCssFileInfo, MediaTypeNames.Text.Css);
        endpoints.MapStaticAsset(indexJsFileInfo, MediaTypeNames.Text.JavaScript);
        endpoints.MapStaticAsset(faviconFileInfo, MediaTypeNames.Image.Icon);

        return endpoints;
    }

    private static void MapDocumentEndpoint(this IEndpointRouteBuilder endpoints, FileInfo documentFileInfo)
    {
        if (!documentFileInfo.Exists) return;

        var fileContent = File.ReadAllText(documentFileInfo.FullName);
        endpoints.MapGet($"{documentFileInfo.Name}", () => Results.Content(fileContent, "text/html"));
    }

    private static void MapStaticAsset(this IEndpointRouteBuilder endpoints, FileInfo asset, string contentType)
    {
        endpoints.MapGet($"{asset.Name}", (HttpContext httpContext) => HandleStaticAsset(asset, contentType, httpContext))
            .AllowAnonymous();
    }

    private static IResult HandleStaticAsset(FileInfo fileInfo, string contentType, HttpContext httpContext)
    {
        httpContext.Response.Headers.CacheControl = "no-cache";

        if (!fileInfo.Exists)
        {
            return Results.NotFound();
        }

        var etag = $"\"{fileInfo.LastWriteTime.Ticks}\"";
        var ifNoneMatch = httpContext.Request.Headers.IfNoneMatch.ToString();
        if (ifNoneMatch == etag)
        {
            return Results.StatusCode(StatusCodes.Status304NotModified);
        }

        var stream = fileInfo.OpenRead();

        return Results.Stream(stream, contentType, entityTag: new EntityTagHeaderValue(etag));
    }
}