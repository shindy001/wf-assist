using Microsoft.AspNetCore.Http;

namespace WfAssist.Workflows;

internal static class Constants
{
    public const string AppName = "WfAssist";
    public const string ApiRoute = "api";
    public static readonly PathString ApiRouteSegment = new($"/api");

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