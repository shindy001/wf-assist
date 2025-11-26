using System.Data;
using System.Text.Json;
using Dapper;
using WfAssist.AspNetCore.Core.Serialization;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

public sealed class ProcessingResultListTypeHandler : SqlMapper.TypeHandler<List<ProcessingResult>>
{
    public override void SetValue(IDbDataParameter parameter, List<ProcessingResult>? value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, DbJsonSerializerOptions.Value);
    }

    public override List<ProcessingResult>? Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<List<ProcessingResult>>(json, DbJsonSerializerOptions.Value);
        }

        return null;
    }
}