using System.Data;
using Microsoft.Data.Sqlite;
using Shared;

namespace WfAssist.Workflows.Infrastructure;

public sealed class SqliteDbConnectionProvider : IDbConnectionProvider
{
    public IDbConnection DbConnection { get; }

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