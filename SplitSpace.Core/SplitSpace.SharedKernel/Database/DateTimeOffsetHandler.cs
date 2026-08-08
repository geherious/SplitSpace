using System.Data;
using Dapper;

namespace SplitSpace.SharedKernel.Database;

public sealed class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override DateTimeOffset Parse(object value)
    {
        // Npgsql returns timestamptz as UTC DateTime (Kind=Utc)
        return value switch
        {
            DateTime dt => new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)),
            DateTimeOffset dto => dto,
            _ => throw new InvalidCastException($"Cannot convert {value.GetType()} to DateTimeOffset")
        };
    }

    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.UtcDateTime;
    }
}
