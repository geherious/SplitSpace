using Grpc.Core;
using Mediator;
using SplitSpace.Finances.Api.SearchService;
using SplitSpace.Finances.Helpers;
using SplitSpace.Finances.Logic.Features.Reports.GetReport;

namespace SplitSpace.Finances.Services;

public class SearchServiceGrpc : SearchService.SearchServiceBase
{
    private readonly IMediator _mediator;

    public SearchServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var dateFrom = request.From.ToDateTimeOffsetOrThrow(request.Timezone, nameof(request.From));
        var dateTo = request.To.ToDateTimeOffsetOrThrow(request.Timezone, nameof(request.To));

        var result = await _mediator.Send(new GetReportCommand(
            spaceId,
            userId,
            dateFrom,
            dateTo), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetReportResponse
        {
            Items = { result.ResultValue.Items.Select(i => new GetReportResponse.Types.ReportItem
            {
                CategoryId = i.CategoryId.ToString(),
                Amount = i.Amount.ToMoney()
            }) }
        };
    }
}
