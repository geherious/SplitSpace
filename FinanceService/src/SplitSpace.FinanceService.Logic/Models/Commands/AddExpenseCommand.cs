namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record AddExpenseCommand(
    Guid SpaceId,
    Guid UserId,
    Guid CategoryId,
    Guid AccountId,
    decimal Amount,
    string Description,
    AddExpenseCommand.ExpenseSplit? Split,
    DateTimeOffset CreatedAt)
{
    public enum ExpenseSplitTemplate
    {
        Equal
    }
    
    public abstract record ExpenseSplit
    {
        private ExpenseSplit() { }

        public sealed record Template(ExpenseSplitTemplate ExpenseSplitTemplate)
            : ExpenseSplit;

        public sealed record Custom()
            : ExpenseSplit;
    }
}
