namespace SplitSpace.Finances.Logic.Features.Expenses.GetExpenses;

public record GetExpensesResultData(IReadOnlyCollection<GetExpensesResultData.Expense> Expenses)
{
    public record ExpenseSplit
    {
        public required IReadOnlyCollection<ExpenseSplitItem> ExpenseSplitItems { get; init; }
    }

    public record ExpenseSplitItem
    {
        public required Guid UserId { get; init; }
        public required decimal AmountToPay { get; init; }
        public required decimal ExpensePercent { get; init; }
    }

    public record Expense(
        Guid ExpenseId,
        Guid CategoryId,
        Guid? BalanceId,
        decimal Amount,
        string Description,
        GetExpensesResultData.ExpenseSplit? Split);
}
