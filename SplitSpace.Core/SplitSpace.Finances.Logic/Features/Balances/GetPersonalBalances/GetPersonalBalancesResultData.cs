namespace SplitSpace.Finances.Logic.Features.Balances.GetPersonalBalances;

public record GetPersonalBalancesResultData(IReadOnlyCollection<GetPersonalBalancesResultData.Account> Accounts)
{
    public record Account
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required decimal Balance { get; init; }
        public required Guid UserId { get; init; }
    }
}
