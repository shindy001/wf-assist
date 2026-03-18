namespace Shared;

public static class Constants
{
    private const string WwwRootDirectory = "wwwroot";

    public const string AppName = "WfAssist";
    public const string AppRoute = "wfAssist";
    public const string ApiRoute = "api";

    // WFAssist js client files
    public const string IndexHtmlFile = "index.html";
    public const string IndexCssFile = "index.css";
    public const string IndexJsFile = "index.js";
    public const string FaviconFile = "favicon.ico";

    public static readonly string ClientRootDirectoryPath = Path.Combine(AppContext.BaseDirectory, WwwRootDirectory, AppRoute);
    public static readonly string SqliteDbConnectionString = GetSqliteConnectionString();

    private static string GetSqliteConnectionString()
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbPath = Path.Combine(folderPath, "WfAssist", "wf-assist.db");

        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        }

        return $"Data Source={dbPath}";
    }
}