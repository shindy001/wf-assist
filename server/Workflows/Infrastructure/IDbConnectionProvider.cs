using System.Data;

namespace WfAssist.Workflows.Infrastructure;

public interface IDbConnectionProvider : IDisposable
{
    IDbConnection DbConnection { get; }
}