using System.Data.Common;

namespace Shared;

public interface IDbConnectionProvider : IDisposable
{
    DbConnection DbConnection { get; }
}