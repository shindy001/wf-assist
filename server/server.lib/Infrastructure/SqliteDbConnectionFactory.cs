using System.Data;
using Microsoft.Data.Sqlite;

namespace WfAssist.AspNetCore.Infrastructure;

public interface IDbConnectionFactory
{
    public IDbConnection CreateConnection();
}

internal sealed class SqliteDbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqliteConnection(Constants.SqliteDbConnectionString);
}