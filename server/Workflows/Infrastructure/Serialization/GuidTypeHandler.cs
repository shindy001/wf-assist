using System.Data;
using Dapper;

namespace WfAssist.Workflows.Infrastructure.Serialization;

public sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        return value is Guid guid
            ? guid
            : new Guid((string)value);
    }
}