using System.Data;

namespace WfAssist.AspNetCore.Core;

public interface IDbConnectionProvider : IDisposable
{
    IDbConnection DbConnection { get; }
}