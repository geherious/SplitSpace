namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetDebtsApiResponse
{
    public required List<GetDebtsApiResponseDebt> Debts { get; set; }
}

public class GetDebtsApiResponseDebt
{
    public required string DebtId { get; set; }
    public required string SpaceId { get; set; }
    public required string FromUserId { get; set; }
    public required string ToUserId { get; set; }
    public decimal Amount { get; set; }
}
