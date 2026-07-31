namespace SplitSpace.Finances.Logic.Features.Debts.GetDebts;

public record GetDebtsResultData(IReadOnlyCollection<GetDebtsResultData.Debt> Debts)
{
    public record Debt(
        Guid Id,
        Guid SpaceId,
        Guid FromUserId,
        Guid ToUserId,
        decimal Amount);
}
