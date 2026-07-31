using Dapper;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public sealed class BalanceOwnerTypeHandler : SqlMapper.TypeHandler<BalanceOwnerType>
{
    public override BalanceOwnerType Parse(object value)
    {
        return Enum.Parse<BalanceOwnerType>((string)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, BalanceOwnerType value)
    {
        parameter.Value = value.ToString();
    }
}
