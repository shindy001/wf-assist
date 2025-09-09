using System.Data;
using Microsoft.Extensions.Logging;

namespace WfAssist.AspNetCore.Infrastructure;

public interface IUnitOfWork
{
    // TODO define repositories

    public Task CommitAsync();
}

internal sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;
    private bool _commited;

    public UnitOfWork(IDbConnectionFactory dbConnectionFactory, ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        _dbConnection = dbConnectionFactory.CreateConnection();
        _dbConnection.Open();
        _dbTransaction = _dbConnection.BeginTransaction();

        // TODO create repositories with _dbConnection as param
    }

    public Task CommitAsync()
    {
        if (_dbTransaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            _dbTransaction.Commit();
            _commited = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Transaction commit failed, will try rollback before rethrow.");
            TryRollback();
            throw;
        }

        return Task.CompletedTask;
    }

    private void TryRollback()
    {
        try
        {
            _dbTransaction.Rollback();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Transaction rollback failed.");
        }
        finally
        {
            _dbTransaction.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        if (!_commited)
        {
            TryRollback();
        }

        _dbTransaction.Dispose();
        _dbConnection.Dispose();

        return ValueTask.CompletedTask;
    }
}