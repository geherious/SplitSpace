using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class BalanceReadRepository : IBalanceReadRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public BalanceReadRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal BalanceReadRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task<IReadOnlyCollection<BalanceEntity>> GetSpaceBalanceBatchAsync(SpaceId spaceId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                total,
                owner_type AS OwnerType,
                owner_id AS OwnerId,
                created_by as CreatedBy
            FROM balance
            WHERE owner_type = @OwnerType AND owner_id = @OwnerId
            """;

        var parameters = new { OwnerType = nameof(BalanceOwnerType.Space), OwnerId = spaceId.Value };

        if (_transaction is not null)
        {
            return (await _transaction.Connection!.QueryAsync<BalanceEntity>(sql, parameters, _transaction)).AsList();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<BalanceEntity>(sql, parameters)).AsList();
    }

    public async Task<IReadOnlyCollection<BalanceEntity>> GetUserBalanceBatchAsync(UserId userId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                total,
                owner_type AS OwnerType,
                owner_id AS OwnerId,
                created_by as CreatedBy
            FROM balance
            WHERE owner_type = @OwnerType AND owner_id = @OwnerId
            """;

        var parameters = new { OwnerType = nameof(BalanceOwnerType.Personal), OwnerId = userId.Value };

        if (_transaction is not null)
        {
            return (await _transaction.Connection!.QueryAsync<BalanceEntity>(sql, parameters, _transaction)).ToArray();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<BalanceEntity>(sql, parameters)).ToArray();
    }
}
