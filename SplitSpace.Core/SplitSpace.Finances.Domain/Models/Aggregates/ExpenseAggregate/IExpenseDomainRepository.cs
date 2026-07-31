namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

public interface IExpenseDomainRepository
{
    Task SaveAsync(Expense expense, CancellationToken cancellationToken);
}
