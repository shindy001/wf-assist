using System.Data;
using Dapper;
using WfAssist.AspNetCore.Core;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

public sealed class WorkflowRepository
{
    private readonly IDbConnection _dbConnection;

    public WorkflowRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnection = dbConnectionProvider.DbConnection;
    }

    public async Task<IEnumerable<WorkflowIdentity>> GetIdentities()
    {
        const string sql = "SELECT id, name FROM Workflows";

        return await _dbConnection.QueryAsync<WorkflowIdentity>(sql);
    }

    public async Task<Workflow?> GetById(Guid id)
    {
        const string sql = "SELECT * FROM Workflows WHERE Id = @Id";

        return await _dbConnection.QuerySingleOrDefaultAsync<Workflow>(sql, new { Id = id });
    }

    public async Task<bool> Exists(Guid id)
    {
        const string sql = "SELECT COUNT(1) FROM Workflows WHERE Id = @Id";

        return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task Create(Workflow workflow)
    {
        const string sql = "INSERT INTO Workflows (Id, Name, Data) VALUES (@Id, @Name, @Data)";

        await _dbConnection.ExecuteAsync(sql, workflow);
    }

    public async Task Rename(Guid id, string newName)
    {
        const string sql = "UPDATE Workflows SET Name = @Name WHERE Id = @Id";

        await _dbConnection.ExecuteAsync(sql, new {Id = id, Name = newName});
    }

    public async Task UpdateData(Guid id, WorkflowData data)
    {
        const string sql = "UPDATE Workflows SET Data = @Data WHERE Id = @Id";

        await _dbConnection.ExecuteAsync(sql, new { Id = id, Data = data});
    }

    public async Task Delete(Guid id)
    {
        const string sql = "DELETE FROM Workflows WHERE Id = @Id";

        if (await Exists(id))
        {
            await _dbConnection.ExecuteAsync(sql, new {Id = id});
        }
    }
}