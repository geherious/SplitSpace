using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class ExpenseSplitRepository : IExpenseSplitRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public ExpenseSplitRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal ExpenseSplitRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task AddAsync(IReadOnlyCollection<ExpenseSplit> expenseSplits, CancellationToken ct)
    {
        if (expenseSplits.Count == 0)
            return;

        const string sql =
            """
            INSERT INTO expense_split (id, expense_id, space_id, user_id, amount_to_pay)
            VALUES (@Id, @ExpenseId, @SpaceId, @UserId, @AmountToPay)
            """;

        var parameters = expenseSplits
            .Select(split => new
            {
                Id = split.Id,
                split.ExpenseId,
                split.SpaceId,
                split.UserId,
                split.AmountToPay
            });

        if (_transaction is not null)
        {
            foreach (var parameter in parameters)
            {
                await _transaction.Connection!.ExecuteAsync(sql, parameter, _transaction);
            }
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        foreach (var parameter in parameters)
        {
            await connection.ExecuteAsync(sql, parameter);
        }
    }

    public async Task<ExpenseSplit?> GetAsync(Guid spaceId, Guid expenseId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                expense_id AS ExpenseId,
                space_id AS SpaceId,
                user_id AS UserId,
                amount_to_pay AS AmountToPay
            FROM expense_split
            WHERE space_id = @SpaceId AND expense_id = @ExpenseId
            """;

        var parameters = new { SpaceId = spaceId, ExpenseId = expenseId };

        if (_transaction is not null)
        {
            return await _transaction.Connection!.QuerySingleOrDefaultAsync<ExpenseSplit>(sql, parameters, _transaction);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return await connection.QuerySingleOrDefaultAsync<ExpenseSplit>(sql, parameters);
    }
}
