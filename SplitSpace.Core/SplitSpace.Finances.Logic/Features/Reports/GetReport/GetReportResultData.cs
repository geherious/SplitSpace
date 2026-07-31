namespace SplitSpace.Finances.Logic.Features.Reports.GetReport;

public record GetReportResultData(IReadOnlyCollection<GetReportResultData.GetReportItem> Items)
{
    public record GetReportItem(Guid CategoryId, decimal Amount);
}
