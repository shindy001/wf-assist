using System.Data;
using Dapper;
using WfAssist.AspNetCore.Core;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

public sealed class ExecutionRepository
{
    private readonly IDbConnection _dbConnection;

    public ExecutionRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnection = dbConnectionProvider.DbConnection;
    }

    public async Task<IEnumerable<ExecutionIdentity>> GetAllRuns()
    {
        const string sql = "SELECT id, status FROM Executions";

        return await _dbConnection.QueryAsync<ExecutionIdentity>(sql);
    }

    public async Task<Execution?> GetById(Guid runId)
    {
        const string sql = "SELECT * FROM Executions WHERE Id = @Id";

        return await _dbConnection.QuerySingleOrDefaultAsync<Execution>(sql, new { Id = runId });
    }

    public async Task Delete(Guid runId)
    {
        const string sql = "DELETE FROM Executions WHERE Id = @Id";

        if (await Exists(runId))
        {
            await _dbConnection.ExecuteAsync(sql, new {Id = runId});
        }
    }

    public async Task<Execution?> GetQueuedRun()
    {
        const string sql = "SELECT * FROM Executions WHERE Status = @Status";

        return await _dbConnection.QueryFirstOrDefaultAsync<Execution>(sql, new { Status = ExecutionStatus.Queued });
    }

    public async Task<Guid> AddRun(Execution run)
    {
        const string sql =
            "INSERT INTO Executions (Id, Status, Snapshot, ProcessingResults) VALUES (@Id, @Status, @Snapshot, @ProcessingResults)";

        await _dbConnection.ExecuteAsync(sql,
            new {Id = run.Id, Status = run.Status, Snapshot = run.Snapshot, ProcessingResults = run.ProcessingResults});

        return run.Id;
    }

    public async Task CompleteRun(Guid runId, ExecutionStatus status, Dictionary<string, ProcessingResult> processingResults)
    {
        const string sql = "UPDATE Executions SET Status = @Status, ProcessingResults = @ProcessingResults WHERE Id = @Id";

        await _dbConnection.ExecuteAsync(sql, new { Id = runId, Status = status, ProcessingResults = processingResults });
    }

    public async Task UpdateRunStatus(Guid runId, ExecutionStatus status)
    {
        const string sql = "UPDATE Executions SET Status = @Status WHERE Id = @Id";

        await _dbConnection.ExecuteAsync(sql, new { Id = runId, Status = status });
    }

    private async Task<bool> Exists(Guid runId)
    {
        const string sql = "SELECT COUNT(1) FROM Executions WHERE Id = @Id";

        return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Id = runId });
    }
}