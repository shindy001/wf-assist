using System.Data;
using System.Text.Json;
using Dapper;
using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Infrastructure.Serialization;

public sealed class ProcessingResultDictionaryTypeHandler : SqlMapper.TypeHandler<Dictionary<string, ProcessingResult>>
{
    public override void SetValue(IDbDataParameter parameter, Dictionary<string, ProcessingResult>? value)
    {
        parameter.Value = value is null
            ? DBNull.Value
            : JsonSerializer.Serialize(value, DbJsonSerializerOptions.Value);
    }

    public override Dictionary<string, ProcessingResult>? Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<Dictionary<string, ProcessingResult>>(json, DbJsonSerializerOptions.Value);
        }

        return null;
    }
}