using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Models;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class ExpenseRepository : IExpenseRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public ExpenseRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal ExpenseRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task AddAsync(Expense expense, CancellationToken ct)
    {
        const string sql =
            """
            INSERT INTO expense (id, space_id, created_by, category_id, balance_id, amount, description, created_at)
            VALUES (@Id, @SpaceId, @CreatedBy, @CategoryId, @BalanceId, @Amount, @Description, @CreatedAt)
            """;

        var parameters = new
        {
            expense.Id,
            expense.SpaceId,
            expense.CreatedBy,
            expense.CategoryId,
            expense.BalanceId,
            expense.Amount,
            expense.Description,
            expense.CreatedAt
        };

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, parameters, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await connection.ExecuteAsync(sql, parameters);
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
            var expenses = (await _transaction.Connection!.QueryAsync<Expense>(expenseSql, new { SpaceId = spaceId }, _transaction)).ToArray();
            return await AssembleAsync(expenses);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var result = (await connection.QueryAsync<Expense>(expenseSql, new { SpaceId = spaceId })).ToArray();
        return await AssembleAsync(connection, result);
    }

    public async Task<Expense?> GetAsync(Guid spaceId, Guid expenseId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                created_by AS CreatedBy,
                category_id AS CategoryId,
                balanceId AS BalanceId,
                amount,
                description,
                created_at AS CreatedAt
            FROM expense
            WHERE id = @Id AND space_id = @SpaceId
            """;

        var parameters = new { Id = expenseId, SpaceId = spaceId };

        if (_transaction is not null)
        {
            return await _transaction.Connection!.QuerySingleOrDefaultAsync<Expense>(sql, parameters, _transaction);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        return await connection.QuerySingleOrDefaultAsync<Expense>(sql, parameters);
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
        IReadOnlyCollection<Expense> expenses)
    {
        if (expenses.Count == 0)
            return [];

        const string sql =
            """
            SELECT
                id,
                name,
                balance,
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
        var splits = (await connection.QueryAsync<ExpenseSplit>(splitSql, new { ExpenseIds = expenses.Select(e => e.Id).ToArray() }))
            .ToArray();

        return expenses
            .Select(e => new ExpenseSplitAggregate(
                e,
                balances[e.BalanceId],
                splits.Where(s => s.ExpenseId == e.Id).ToArray()))
            .ToArray();
    }

    private async Task<IReadOnlyCollection<ExpenseSplitAggregate>> AssembleAsync(IReadOnlyCollection<Expense> expenses)
    {
        return await AssembleAsync(_transaction!.Connection!, expenses);
    }
}
