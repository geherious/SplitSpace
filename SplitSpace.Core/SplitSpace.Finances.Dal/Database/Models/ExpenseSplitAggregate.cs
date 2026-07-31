using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Models;

public record ExpenseSplitAggregate(Expense Expense, BalanceEntity BalanceEntity, IReadOnlyCollection<ExpenseSplit> Splits);
