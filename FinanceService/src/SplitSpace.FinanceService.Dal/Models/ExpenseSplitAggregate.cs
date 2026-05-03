using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Models;

public record ExpenseSplitAggregate(Expense Expense, Account Account, IReadOnlyCollection<ExpenseSplit> Splits);
