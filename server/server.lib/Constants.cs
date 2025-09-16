using Microsoft.AspNetCore.Http;

namespace WfAssist.AspNetCore;

internal static class Constants
{
    public const string AppName = "WfAssist";
    public const string AppRoute = "wfAssist";
    public static readonly PathString AppRouteSegment = new($"/{AppRoute}");

    public const string WwwRootDirectory = "wwwroot";
    public const string IndexHtmlFile = "index.html";
    public const string IndexCssFile = "index.css";
    public const string IndexJsFile = "index.js";
    public const string FaviconFile = "favicon.ico";

    public static readonly string SqliteDbConnectionString = GetSqliteConnectionString();

    private static string GetSqliteConnectionString()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "data", "wf-assist.db");

        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        }

        return $"Data Source={dbPath}";
    }
}