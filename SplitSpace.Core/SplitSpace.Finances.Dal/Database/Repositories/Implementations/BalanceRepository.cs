using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class BalanceRepository : IBalanceRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public BalanceRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal BalanceRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task AddAsync(BalanceEntity balanceEntity, CancellationToken ct)
    {
        const string sql =
            """
            INSERT INTO balance (id, name, total, owner_type, owner_id)
            VALUES (@Id, @Name, @Total, @OwnerType, @OwnerId)
            """;

        var parameters = new
        {
            balanceEntity.Id,
            balanceEntity.Name,
            balanceEntity.Total,
            balanceEntity.OwnerType,
            balanceEntity.OwnerId
        };

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, parameters, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<BalanceEntity?> GetAsync(Guid balanceId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                balance,
                owner_type AS OwnerType,
                owner_id AS OwnerId
            FROM balance
            WHERE id = @Id
            """;

        if (_transaction is not null)
        {
            return await _transaction.Connection!.QuerySingleOrDefaultAsync<BalanceEntity>(sql, new { Id = balanceId }, _transaction);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return await connection.QuerySingleOrDefaultAsync<BalanceEntity>(sql, new { Id = balanceId });
    }

    public async Task<IReadOnlyCollection<BalanceEntity>> GetSpaceBalanceBatchAsync(Guid spaceId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                balance,
                owner_type AS OwnerType,
                owner_id AS OwnerId
            FROM balance
            WHERE owner_type = @OwnerType AND owner_id = @OwnerId
            """;

        var parameters = new { OwnerType = BalanceOwnerType.Space, OwnerId = spaceId };

        if (_transaction is not null)
        {
            return (await _transaction.Connection!.QueryAsync<BalanceEntity>(sql, parameters, _transaction)).ToArray();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<BalanceEntity>(sql, parameters)).ToArray();
    }

    public async Task<IReadOnlyCollection<BalanceEntity>> GetUserbalanceBatchAsync(Guid userId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                balance,
                owner_type AS OwnerType,
                owner_id AS OwnerId
            FROM balance
            WHERE owner_type = @OwnerType AND owner_id = @OwnerId
            """;

        var parameters = new { OwnerType = BalanceOwnerType.Personal, OwnerId = userId };

        if (_transaction is not null)
        {
            return (await _transaction.Connection!.QueryAsync<BalanceEntity>(sql, parameters, _transaction)).ToArray();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<BalanceEntity>(sql, parameters)).ToArray();
    }

    public async Task UpdateAmountAsync(Guid balanceId, decimal amount, CancellationToken ct)
    {
        const string sql =
            """
            UPDATE balance
            SET balance = balance + @Amount
            WHERE id = @BalanceId
            """;

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, new { BalanceId = balanceId, Amount = amount }, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await connection.ExecuteAsync(sql, new { BalanceId = balanceId, Amount = amount });
    }
}
