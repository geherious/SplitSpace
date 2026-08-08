using SplitSpace.Finances.Dal.Database.Models;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

namespace SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;

public sealed class ExpenseTestRepository
{
    private readonly IExpenseDomainRepository _domainRepository;
    private readonly IExpenseReadRepository _readRepository;

    public ExpenseTestRepository(
        IExpenseDomainRepository domainRepository,
        IExpenseReadRepository readRepository)
    {
        _domainRepository = domainRepository;
        _readRepository = readRepository;
    }

    public Task SaveAsync(Expense expense, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(expense, ct);
    }

    public Task<IReadOnlyCollection<ExpenseSplitAggregate>> GetBatchAsync(Guid spaceId, CancellationToken ct = default)
    {
        return _readRepository.GetBatchAsync(spaceId, ct);
    }

    public Task<IReadOnlyCollection<ExpenseByCategory>> GetGroupedByCategory(
        Guid spaceId,
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        CancellationToken ct = default)
    {
        return _readRepository.GetGroupedByCategory(spaceId, fromDate, toDate, ct);
    }
}
