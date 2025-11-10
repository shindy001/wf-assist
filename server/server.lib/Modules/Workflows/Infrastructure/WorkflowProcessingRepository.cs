using System.Data;
using Dapper;
using WfAssist.AspNetCore.Core;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

public sealed class WorkflowProcessingRepository
{
    private readonly IDbConnection _dbConnection;

    public WorkflowProcessingRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnection = dbConnectionProvider.DbConnection;
    }

    public async Task<IEnumerable<WorkflowRunIdentity>> GetAllRuns()
    {
        const string sql = "SELECT id, status FROM WorkflowRuns";

        return await _dbConnection.QueryAsync<WorkflowRunIdentity>(sql);
    }

    public async Task<WorkflowRun?> GetById(Guid runId)
    {
        const string sql = "SELECT * FROM WorkflowRuns WHERE Id = @Id";

        return await _dbConnection.QuerySingleOrDefaultAsync<WorkflowRun>(sql, new { Id = runId });
    }

    public async Task Delete(Guid runId)
    {
        const string sql = "DELETE FROM WorkflowRuns WHERE Id = @Id";

        if (await Exists(runId))
        {
            await _dbConnection.ExecuteAsync(sql, new {Id = runId});
        }
    }

    public async Task<Guid> QueueRun(WorkflowRun run)
    {
        const string sql = "INSERT INTO WorkflowRuns (Id, Status, Snapshot) VALUES (@Id, @Status, @Snapshot)";

        await _dbConnection.ExecuteAsync(sql, new { Id = run.Id, Status = run.Status, Snapshot = run.Snapshot });

        return run.Id;
    }

    private async Task<bool> Exists(Guid runId)
    {
        const string sql = "SELECT COUNT(1) FROM WorkflowRuns WHERE Id = @Id";

        return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Id = runId });
    }
}