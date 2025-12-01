using System.Data;
using System.Text.Json;
using Dapper;
using WfAssist.AspNetCore.Core.Serialization;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure.Serialization;

public sealed class WorkflowDataTypeHandler : SqlMapper.TypeHandler<WorkflowData>
{
    public override void SetValue(IDbDataParameter parameter, WorkflowData? value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, DbJsonSerializerOptions.Value);
    }

    public override WorkflowData? Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<WorkflowData>(json, DbJsonSerializerOptions.Value);
        }

        return null;
    }
}