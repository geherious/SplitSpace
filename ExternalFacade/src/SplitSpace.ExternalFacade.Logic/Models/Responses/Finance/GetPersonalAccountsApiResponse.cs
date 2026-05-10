using Google.Protobuf.WellKnownTypes;

namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetPersonalAccountsApiResponse
{
    public required List<GetPersonalAccountsApiResponseAccount> Accounts { get; set; }
}

public class GetPersonalAccountsApiResponseAccount
{
    public required string AccountId { get; set; }
    public required string Name { get; set; }
    public decimal Balance { get; set; }
}