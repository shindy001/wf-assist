using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared;

internal sealed class SqliteDbConnectionStringProvider : IDbConnectionStringProvider
{
    public string GetConnectionString(string databaseName)
    {
        var dbPath = Path.Combine(Constants.SqliteDbDirectoryPath, $"{databaseName}.db");
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath
        };

        return builder.ConnectionString;
    }
}

public static class SqliteExtensions
{
    public static void AddSqliteDbConnectionStringProvider(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionStringProvider, SqliteDbConnectionStringProvider>();
    }
}