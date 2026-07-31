using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class DebtRepository : IDebtRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public DebtRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal DebtRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task AddOrUpdateAsync(IReadOnlyCollection<Debt> debts, CancellationToken ct)
    {
        if (debts.Count == 0)
            return;

        const string sql =
            """
            WITH input AS (
                SELECT *
                FROM UNNEST(
                    @ids::uuid[],
                    @space_ids::uuid[],
                    @from_user_ids::uuid[],
                    @to_user_ids::uuid[],
                    @amounts::numeric[]
                ) AS t(id, space_id, from_user_id, to_user_id, amount)
            )
            INSERT INTO debt (id, space_id, from_user_id, to_user_id, amount)
            SELECT id, space_id, from_user_id, to_user_id, amount
            FROM input
            ON CONFLICT (space_id, from_user_id, to_user_id) DO UPDATE SET
                amount = debt.amount + EXCLUDED.amount
            """;

        var parameters = new
        {
            ids = debts.Select(d => d.Id).ToArray(),
            space_ids = debts.Select(d => d.SpaceId).ToArray(),
            from_user_ids = debts.Select(d => d.FromUserId).ToArray(),
            to_user_ids = debts.Select(d => d.ToUserId).ToArray(),
            amounts = debts.Select(d => d.Amount).ToArray()
        };

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, parameters, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<IReadOnlyCollection<Debt>> GetBatchAsync(Guid spaceId, Guid userId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                from_user_id AS FromUserId,
                to_user_id AS ToUserId,
                amount
            FROM debt
            WHERE space_id = @SpaceId AND (from_user_id = @UserId OR to_user_id = @UserId)
            """;

        var parameters = new { SpaceId = spaceId, UserId = userId };

        if (_transaction is not null)
        {
            return (await _transaction.Connection!.QueryAsync<Debt>(sql, parameters, _transaction)).ToArray();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<Debt>(sql, parameters)).ToArray();
    }
}
