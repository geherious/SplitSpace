using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

public interface IExpenseDomainRepository
{
    Task SaveAsync(Expense expense, CancellationToken cancellationToken);
}
