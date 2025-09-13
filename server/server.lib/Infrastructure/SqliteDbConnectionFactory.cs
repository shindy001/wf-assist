using System.Data;
using Microsoft.Data.Sqlite;

namespace WfAssist.AspNetCore.Infrastructure;

public interface IDbConnectionFactory
{
    public IDbConnection CreateConnection();
}

public interface IReadOnlyDbConnectionFactory : IDbConnectionFactory;

internal sealed class SqliteDbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqliteConnection(Constants.SqliteDbConnectionString);
}

internal sealed class SqliteReadOnlyDbConnectionFactory : IReadOnlyDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var builder = new SqliteConnectionStringBuilder(Constants.SqliteDbConnectionString)
        {
            Mode = SqliteOpenMode.ReadOnly
        };

        return new SqliteConnection(builder.ToString());
    }
}