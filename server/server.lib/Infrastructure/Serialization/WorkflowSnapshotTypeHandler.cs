using System.Data;
using System.Text.Json;
using Dapper;
using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Infrastructure.Serialization;

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