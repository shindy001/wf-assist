using System.Data;
using WfAssist.AspNetCore.Shared;

namespace WfAssist.AspNetCore.Infrastructure;

public sealed class DbConnectionProvider : IDbConnectionProvider
{
    public IDbConnection DbConnection { get; }

    public DbConnectionProvider(IDbConnectionFactory dbConnectionFactory)
    {
        DbConnection = dbConnectionFactory.CreateConnection();
        DbConnection.Open();
    }

    public void Dispose()
    {
        DbConnection.Dispose();
    }
}