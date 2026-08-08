using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class DebtReadRepository : IDebtReadRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public DebtReadRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal DebtReadRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task<IReadOnlyCollection<DebtEntity>> GetBatchAsync(Guid spaceId, Guid userId, CancellationToken ct)
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
            return (await _transaction.Connection!.QueryAsync<DebtEntity>(sql, parameters, _transaction)).ToArray();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<DebtEntity>(sql, parameters)).ToArray();
    }
}
