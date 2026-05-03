namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record ExpenseSplit
{
    public required Guid Id { get; init; }
    public required Guid ExpenseId { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid UserId { get; init; }
    public required decimal AmountToPay { get; init; }
}
