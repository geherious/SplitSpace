namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record ExpenseTag
{
    public required Guid Id { get; init; }
    public required Guid ExpenseId { get; init; }
    public required Guid TagId { get; init; }
}
