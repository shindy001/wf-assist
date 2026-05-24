using System.Data.Common;
using Microsoft.Data.Sqlite;
using WfAssist.Shared;

namespace WfAssist.AspNetCore;

public sealed class SqliteDbConnectionStringProvider : IDbConnectionStringProvider
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