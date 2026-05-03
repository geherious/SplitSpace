using Grpc.Core;
using SplitSpace.FinanceService.Api.SearchService;
using SplitSpace.FinanceService.Helpers;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Services;

namespace SplitSpace.FinanceService.Services;

public class SearchServiceGrpc : SearchService.SearchServiceBase
{
    private readonly ISearchService _searchService;

    public SearchServiceGrpc(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var dateFrom = request.From.ToDateTimeOffsetOrThrow(request.Timezone, nameof(request.From));
        var dateTo = request.To.ToDateTimeOffsetOrThrow(request.Timezone, nameof(request.To));

        var result = await _searchService.GetReportAsync(new GetReportCommand(
            spaceId,
            userId,
            dateFrom,
            dateTo));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetReportResponse
        {
            Items = { result.Value.items.Select(i => new GetReportResponse.Types.ReportItem
            {
                CategoryId = i.CategoryId.ToString(),
                Amount = i.Amount.ToMoney()
            }) }
        };
    }
}