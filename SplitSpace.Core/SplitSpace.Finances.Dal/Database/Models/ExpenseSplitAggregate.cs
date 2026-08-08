using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Models;

public record ExpenseSplitAggregate(ExpenseEntity ExpenseEntity, BalanceEntity BalanceEntity, IReadOnlyCollection<ExpenseSplitEntity> Splits);
