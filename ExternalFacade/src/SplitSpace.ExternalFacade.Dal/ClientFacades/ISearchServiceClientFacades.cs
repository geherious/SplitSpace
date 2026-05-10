using SplitSpace.FinanceService.Api.SearchService;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades;

public interface ISearchServiceClientFacades
{
    Task<GetReportResponse> GetReportAsync(GetReportRequest request, CancellationToken cancellationToken);
}
