using System.Data;
using Microsoft.Data.Sqlite;

namespace WfAssist.AspNetCore.Infrastructure;

public interface IDbConnectionFactory
{
    public IDbConnection CreateConnection();
    public IDbConnection CreateReadonlyConnection();
}

internal sealed class SqliteDbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqliteConnection(Constants.SqliteDbConnectionString);
    public IDbConnection CreateReadonlyConnection()
    {
        var builder = new SqliteConnectionStringBuilder(Constants.SqliteDbConnectionString)
        {
            Mode = SqliteOpenMode.ReadOnly
        };

        return new SqliteConnection(builder.ToString());
    }
}