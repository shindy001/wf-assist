using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace WfAssist.AspNetCore;

internal static class ClientEndpoints
{
    /// <summary>
    /// 1. Registers WfAssist Client app resources via endpoints<br/>
    /// 2. WfAssist client app and this lib project is bundled to a nuget in nuget project (on project build) in solution.
    /// Nuget package output dir is [solutionDir/nuget/packages]<br/>
    /// 3. If you are not using the nuget, you need to copy dist binaries to [your server host outputDir]/wwwroot/wfAssist<br/>
    /// Example path - "_AspNetCore.Host/bin/debug/net10.0/wwwroot/wfAssist"<br/>
    /// or run wfAssist client app directly - viz. package.json in client project
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="logger"></param>
    public static void MapWfAssistClientEndpoints(this IEndpointRouteBuilder endpoints, ILogger logger)
    {
        // Redirect to client index - wfAssist files (from nuget or dist) must be in [server host outputDir]/wwwroot/wfAssist
        endpoints.MapGet($"/", context =>
        {
            context.Response.Redirect($"/{Constants.AppRoute}/{Constants.IndexHtmlFile}");
            return Task.CompletedTask;
        }).ExcludeFromDescription();

        MapWfAssistClientResources(endpoints, logger);
    }

    /// <summary>
    /// Maps WfAssist client resources and provides them via endpoints, client resources are copies to output only when used via nuget.
    /// Nuget project bundles this WfAssist.AspNetCore and client binaries and copy them to output directory when the project using the nuget is build.
    /// </summary>
    /// <param name="routeGroup"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    private static void MapWfAssistClientResources(IEndpointRouteBuilder routeGroup, ILogger logger)
    {
        var rootDirectoryInfo = new DirectoryInfo(Constants.ClientRootDirectoryPath);
        if (!rootDirectoryInfo.Exists)
        {
            var errorMessage =
                $"""
                  *****
                  Failed to register WFAssist UI, web application binaries not found in directory: '{Constants.ClientRootDirectoryPath}'.
                  If your are developing or debugging WFAssist server and need the client UI, copy built client UI binaries to the specified directory.
                  Otherwise only server API will be available.
                  (You can ignore this message if you are only using the API or running WFAssist UI as separate process.)
                  *****
                  """;

            logger.LogError("{errorMessage}", errorMessage);

            return;
        }

        var indexHtmlFileInfo = rootDirectoryInfo.GetFiles(Constants.IndexHtmlFile).SingleOrDefault();
        var indexCssFileInfo = rootDirectoryInfo.GetFiles(Constants.IndexCssFile).SingleOrDefault();
        var indexJsFileInfo = rootDirectoryInfo.GetFiles(Constants.IndexJsFile).SingleOrDefault();
        var faviconFileInfo = rootDirectoryInfo.GetFiles(Constants.FaviconFile).SingleOrDefault();

        if (indexHtmlFileInfo is null
            || indexCssFileInfo is null
            || indexJsFileInfo is null
            || faviconFileInfo is null)
        {
            var errorMessage =
                $"""
                  *****
                  Failed to register WFAssist UI, some files are missing in app root directory '{Constants.ClientRootDirectoryPath}':
                  {Constants.IndexHtmlFile}: {(indexHtmlFileInfo is null ? "missing": "found")}
                  {Constants.IndexCssFile}: {(indexCssFileInfo is null ? "missing": "found")}
                  {Constants.IndexJsFile}: {(indexJsFileInfo is null ? "missing": "found")}
                  {Constants.FaviconFile}: {(faviconFileInfo is null ? "missing": "found")}
                  
                  If your are developing or debugging WFAssist server and need the client UI, copy built client UI binaries to the specified directory.
                  Otherwise only server API will be available.
                  (You can ignore this message if you are only using the API or running WFAssist UI as separate process.)
                  *****
                  """;

            logger.LogError("{errorMessage}", errorMessage);

            return;
        }

        routeGroup.MapDocumentEndpoint(indexHtmlFileInfo);
        routeGroup.MapStaticAsset(indexCssFileInfo, MediaTypeNames.Text.Css);
        routeGroup.MapStaticAsset(indexJsFileInfo, MediaTypeNames.Text.JavaScript);
        routeGroup.MapStaticAsset(faviconFileInfo, MediaTypeNames.Image.Icon);
    }

    private static void MapDocumentEndpoint(this IEndpointRouteBuilder endpoints, FileInfo documentFileInfo)
    {
        if (!documentFileInfo.Exists) return;

        var fileContent = File.ReadAllText(documentFileInfo.FullName);
        endpoints
            .MapGet($"{documentFileInfo.Name}", () => Results.Content(fileContent, "text/html"))
            // Documents should be excluded from openapi by default
            .ExcludeFromDescription();
    }

    private static void MapStaticAsset(this IEndpointRouteBuilder endpoints, FileInfo asset, string contentType)
    {
        endpoints
            .MapGet($"{asset.Name}", (HttpContext httpContext) => HandleStaticAsset(asset, contentType, httpContext))
            // Static assets should be excluded from openapi by default
            .ExcludeFromDescription();
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