namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetCategoryReportApiResponse
{
    public required List<GetCategoryReportApiResponseReportItem> Items { get; set; }
}

public class GetCategoryReportApiResponseReportItem
{
    public required Guid CategoryId { get; set; }
    public required decimal Amount { get; set; }
}
