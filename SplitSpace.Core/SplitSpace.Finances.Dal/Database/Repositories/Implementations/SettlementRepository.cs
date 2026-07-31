using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class SettlementRepository : ISettlementRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public SettlementRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal SettlementRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task AddAsync(Settlement settlement, CancellationToken ct)
    {
        const string sql =
            """
            INSERT INTO settlement (id, from_user_id, from_balance_id, to_user_id, amount, created_at)
            VALUES (@Id, @FromUserId, @FromBalanceId, @ToUserId, @Amount, @CreatedAt)
            """;

        var parameters = new
        {
            settlement.Id,
            settlement.FromUserId,
            settlement.FromBalanceId,
            settlement.ToUserId,
            settlement.Amount,
            settlement.CreatedAt
        };

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, parameters, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await connection.ExecuteAsync(sql, parameters);
    }
}
