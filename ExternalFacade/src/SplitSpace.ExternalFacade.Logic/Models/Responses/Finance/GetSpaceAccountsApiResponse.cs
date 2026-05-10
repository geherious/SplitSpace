using Google.Protobuf.WellKnownTypes;

namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetSpaceAccountsApiResponse
{
    public required List<GetSpaceAccountsApiResponseAccount> Accounts { get; set; }
}

public class GetSpaceAccountsApiResponseAccount
{
    public required string AccountId { get; set; }
    public required string Name { get; set; }
    public decimal Balance { get; set; }
    public required string SpaceId { get; set; }
}
