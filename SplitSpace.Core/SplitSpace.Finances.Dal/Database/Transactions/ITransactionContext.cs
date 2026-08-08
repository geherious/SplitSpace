using System.Data;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;

namespace SplitSpace.Finances.Dal.Database.Transactions;

public interface ITransactionContext : IAsyncDisposable
{
    RepositoryRegistry Repositories { get; }

    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);

    public record RepositoryRegistry
    {
        public required IBalanceReadRepository BalanceReadRepository { get; init; }

        public required ICategoryDomainRepository CategoryRepository { get; init; }

        public required IDebtReadRepository DebtReadRepository { get; init; }

        public required IExpenseReadRepository ExpenseReadRepository { get; init; }

        public required IBalanceDomainRepository BalanceDomainRepository { get; init; }

        public required IDebtDomainRepository DebtDomainRepository { get; init; }

        public required ISettlementDomainRepository SettlementDomainRepository { get; init; }
    }
}
