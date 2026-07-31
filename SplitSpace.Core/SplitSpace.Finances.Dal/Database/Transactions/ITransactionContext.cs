using System.Data;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;

namespace SplitSpace.Finances.Dal.Database.Transactions;

public interface ITransactionContext : IAsyncDisposable
{
    RepositoryRegistry Repositories { get; }

    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);

    public record RepositoryRegistry
    {
        public required IBalanceRepository BalanceRepository { get; init; }

        public required ICategoryDomainRepository CategoryRepository { get; init; }

        public required IDebtRepository DebtRepository { get; init; }

        public required IExpenseRepository ExpenseRepository { get; init; }

        public required IExpenseSplitRepository ExpenseSplitRepository { get; init; }

        public required ISettlementRepository SettlementRepository { get; init; }
    }
}
