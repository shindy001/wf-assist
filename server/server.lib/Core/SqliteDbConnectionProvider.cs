using System.Data;
using Microsoft.Data.Sqlite;

namespace WfAssist.AspNetCore.Core;

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