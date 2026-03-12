namespace WfAssist.AspNetCore;

internal static class Constants
{
    public const string AppName = "WfAssist";
    public const string AppRoute = "wfAssist";

    public const string WwwRootDirectory = "wwwroot";
    public const string IndexHtmlFile = "index.html";
    public const string IndexCssFile = "index.css";
    public const string IndexJsFile = "index.js";
    public const string FaviconFile = "favicon.ico";

    public static readonly string ClientRootDirectoryPath = Path.Combine(AppContext.BaseDirectory, WwwRootDirectory, AppRoute);
}