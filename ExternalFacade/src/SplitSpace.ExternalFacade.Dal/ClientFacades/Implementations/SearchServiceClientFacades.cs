using SplitSpace.FinanceService.Api.SearchService;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades.Implementations;

public class SearchServiceClientFacades : ISearchServiceClientFacades
{
    private readonly SearchService.SearchServiceClient _searchServiceClient;

    public SearchServiceClientFacades(SearchService.SearchServiceClient searchServiceClient)
    {
        _searchServiceClient = searchServiceClient;
    }

    public async Task<GetReportResponse> GetReportAsync(GetReportRequest request, CancellationToken cancellationToken)
    {
        var response = await _searchServiceClient.GetReportAsync(request, cancellationToken: cancellationToken);
        return response;
    }
}
