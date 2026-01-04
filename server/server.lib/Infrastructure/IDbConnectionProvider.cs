using System.Data;

namespace WfAssist.AspNetCore.Infrastructure;

public interface IDbConnectionProvider : IDisposable
{
    IDbConnection DbConnection { get; }
}