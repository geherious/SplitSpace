using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Models;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class ExpenseReadRepository : IExpenseReadRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public ExpenseReadRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal ExpenseReadRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task<IReadOnlyCollection<ExpenseSplitAggregate>> GetBatchAsync(Guid spaceId, CancellationToken ct)
    {
        const string expenseSql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                created_by AS CreatedBy,
                category_id AS CategoryId,
                balance_id AS BalanceId,
                amount,
                description,
                created_at AS CreatedAt
            FROM expense
            WHERE space_id = @SpaceId
            """;

        if (_transaction is not null)
        {
            var expenses = (await _transaction.Connection!.QueryAsync<ExpenseEntity>(expenseSql, new { SpaceId = spaceId }, _transaction)).ToArray();
            return await AssembleAsync(expenses);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var result = (await connection.QueryAsync<ExpenseEntity>(expenseSql, new { SpaceId = spaceId })).ToArray();
        return await AssembleAsync(connection, result);
    }

    public async Task<IReadOnlyCollection<ExpenseByCategory>> GetGroupedByCategory(
        Guid spaceId,
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        CancellationToken ct)
    {
        const string sql =
            """
            SELECT category_id AS CategoryId, SUM(amount) AS Amount
            FROM expense
            WHERE space_id = @SpaceId AND created_at >= @FromDate AND created_at < @ToDate
            GROUP BY category_id
            """;

        var parameters = new { SpaceId = spaceId, FromDate = fromDate, ToDate = toDate };

        if (_transaction is not null)
        {
            return (await _transaction.Connection!.QueryAsync<ExpenseByCategory>(sql, parameters, _transaction)).ToArray();
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return (await connection.QueryAsync<ExpenseByCategory>(sql, parameters)).ToArray();
    }

    private async Task<IReadOnlyCollection<ExpenseSplitAggregate>> AssembleAsync(
        NpgsqlConnection connection,
        IReadOnlyCollection<ExpenseEntity> expenses)
    {
        if (expenses.Count == 0)
            return [];

        const string sql =
            """
            SELECT
                id,
                name,
                total,
                owner_type AS OwnerType,
                owner_id AS OwnerId
            FROM balance
            WHERE id = ANY(@BalanceIds)
            """;

        const string splitSql =
            """
            SELECT
                id,
                expense_id AS ExpenseId,
                space_id AS SpaceId,
                user_id AS UserId,
                amount_to_pay AS AmountToPay
            FROM expense_split
            WHERE expense_id = ANY(@ExpenseIds)
            """;

        var balances = (await connection.QueryAsync<BalanceEntity>(sql, new { BalanceIds = expenses.Select(e => e.BalanceId).ToArray() }))
            .ToDictionary(a => a.Id);
        var splits = (await connection.QueryAsync<ExpenseSplitEntity>(splitSql, new { ExpenseIds = expenses.Select(e => e.Id).ToArray() }))
            .ToArray();

        return expenses
            .Select(e => new ExpenseSplitAggregate(
                e,
                balances[e.BalanceId],
                splits.Where(s => s.ExpenseId == e.Id).ToArray()))
            .ToArray();
    }

    private async Task<IReadOnlyCollection<ExpenseSplitAggregate>> AssembleAsync(IReadOnlyCollection<ExpenseEntity> expenses)
    {
        return await AssembleAsync(_transaction!.Connection!, expenses);
    }
}
