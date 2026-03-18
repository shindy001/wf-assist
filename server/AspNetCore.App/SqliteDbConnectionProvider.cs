using System.Data.Common;
using Microsoft.Data.Sqlite;
using Shared;

namespace WfAssist.AspNetCore;

public sealed class SqliteDbConnectionProvider : IDbConnectionProvider
{
    public DbConnection DbConnection { get; }

    public SqliteDbConnectionProvider()
    {
        DbConnection = new SqliteConnection(Constants.SqliteDbConnectionString);
        DbConnection.Open();
    }

    public void Dispose()
    {
        DbConnection.Dispose();
    }
}