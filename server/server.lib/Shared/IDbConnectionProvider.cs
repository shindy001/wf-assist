using System.Data;

namespace WfAssist.AspNetCore.Shared;

public interface IDbConnectionProvider : IDisposable
{
    IDbConnection DbConnection { get; }
}