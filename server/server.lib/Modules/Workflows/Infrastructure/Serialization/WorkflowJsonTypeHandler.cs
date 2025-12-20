using System.Data;
using System.Text.Json;
using Dapper;
using WfAssist.AspNetCore.Core.Serialization;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure.Serialization;

public sealed class WorkflowJsonTypeHandler : SqlMapper.TypeHandler<Workflow>
{
    public override void SetValue(IDbDataParameter parameter, Workflow? value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, DbJsonSerializerOptions.Value);
    }

    public override Workflow? Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<Workflow>(json, DbJsonSerializerOptions.Value);
        }

        return null;
    }
}