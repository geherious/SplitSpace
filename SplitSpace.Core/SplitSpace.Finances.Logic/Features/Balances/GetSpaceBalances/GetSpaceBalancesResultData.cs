namespace SplitSpace.Finances.Logic.Features.Balances.GetSpaceBalances;

public record GetSpaceBalancesResultData(IReadOnlyCollection<GetSpaceBalancesResultData.Balance> Accounts)
{
    public record Balance
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required decimal Total { get; init; }
        public required Guid SpaceId { get; init; }
    }
}
