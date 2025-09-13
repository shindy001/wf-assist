using System.Data;
using Microsoft.Extensions.Logging;

namespace WfAssist.AspNetCore.Infrastructure;

public interface IReadOnlyUnitOfWork
{
    // TODO define repositories
}

internal sealed class ReadOnlyUnitOfWork : IReadOnlyUnitOfWork, IDisposable
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IDbConnection _dbConnection;

    public ReadOnlyUnitOfWork(IReadOnlyDbConnectionFactory dbConnectionFactory, ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        _dbConnection = dbConnectionFactory.CreateConnection();
        _dbConnection.Open();

        // TODO create repositories with _dbConnection as param
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }
}

public interface IUnitOfWork
{
    IDbTransaction BeginTransactionAsync();

    // TODO define repositories
}

internal sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IDbConnection _dbConnection;

    public UnitOfWork(IDbConnectionFactory dbConnectionFactory, ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        _dbConnection = dbConnectionFactory.CreateConnection();
        _dbConnection.Open();

        // TODO create repositories with _dbConnection as param
    }

    public IDbTransaction BeginTransactionAsync()
    {
        return _dbConnection.BeginTransaction();
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }
}
