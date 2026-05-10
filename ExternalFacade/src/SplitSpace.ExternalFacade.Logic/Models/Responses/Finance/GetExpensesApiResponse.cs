using Google.Protobuf.WellKnownTypes;

namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetExpensesApiResponse
{
    public required List<GetExpensesApiResponseExpense> Expenses { get; set; }
}

public class GetExpensesApiResponseExpenseSplitItem
{
    public required string UserId { get; set; }
    public decimal AmountToPay { get; set; }
}

public class GetExpensesApiResponseExpenseSplit
{
    public required List<GetExpensesApiResponseExpenseSplitItem> SplitItems { get; set; }
}

public class GetExpensesApiResponseExpense
{
    public required string ExpenseId { get; set; }
    public required string CategoryId { get; set; }
    public required string AccountId { get; set; }
    public decimal Amount { get; set; }
    public required string Description { get; set; }
    public GetExpensesApiResponseExpenseSplit? Split { get; set; }
}
