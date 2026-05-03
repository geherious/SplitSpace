namespace SplitSpace.FinanceService.Logic.Models.Results;

public record GetSpaceAccountsResultData(IReadOnlyCollection<GetSpaceAccountsResultData.Account> Accounts)
{
    public record Account
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required decimal Balance { get; init; }
        public required Guid SpaceId { get; init; }
    }
}
