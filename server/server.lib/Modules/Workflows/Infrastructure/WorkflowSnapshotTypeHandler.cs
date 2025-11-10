using System.Data;
using System.Text.Json;
using Dapper;
using WfAssist.AspNetCore.Core.Serialization;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

public sealed class WorkflowSnapshotTypeHandler : SqlMapper.TypeHandler<WorkflowSnapshot>
{
    public override void SetValue(IDbDataParameter parameter, WorkflowSnapshot? value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, DbJsonSerializerOptions.Value);
    }

    public override WorkflowSnapshot? Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<WorkflowSnapshot>(json, DbJsonSerializerOptions.Value);
        }

        return null;
    }
}