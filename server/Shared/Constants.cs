namespace WfAssist.Shared;

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
    public static readonly string SqliteDbDirectoryPath = GetSqliteDbDirectoryPath();

    private static string GetSqliteDbDirectoryPath()
    {
        var localAppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbDir = Path.Combine(localAppDataDir, "WfAssist");

        if (!Directory.Exists(dbDir))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbDir)!);
        }

        return dbDir;
    }
}