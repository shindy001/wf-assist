using System.Data.Common;

namespace WfAssist.Shared;

public interface IDbConnectionStringProvider
{
    string GetConnectionString(string databaseName);
}