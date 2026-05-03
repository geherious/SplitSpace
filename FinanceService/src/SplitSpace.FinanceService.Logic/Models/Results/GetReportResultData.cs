namespace SplitSpace.FinanceService.Logic.Models.Results;

public record GetReportResultData(IReadOnlyCollection<GetReportResultData.GetReportItem> items)
{
    public record GetReportItem(Guid CategoryId, decimal Amount);
}
