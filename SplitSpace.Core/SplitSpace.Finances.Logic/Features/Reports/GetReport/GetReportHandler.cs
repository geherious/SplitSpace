using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Reports.GetReport;

public class GetReportHandler : IQueryHandler<GetReportCommand, Result<GetReportResultData>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public GetReportHandler(IExpenseRepository expenseRepository, ISpacesServiceClientFacade spacesServiceClientFacade)
    {
        _expenseRepository = expenseRepository;
        _spacesServiceClientFacade = spacesServiceClientFacade;
    }

    public async ValueTask<Result<GetReportResultData>> Handle(GetReportCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(new UserId(command.UserId), ct);
        if (spaceIds.Contains(new SpaceId(command.SpaceId)) is false)
        {
            return Result<GetReportResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        var aggregate = await _expenseRepository.GetGroupedByCategory(
            command.SpaceId,
            command.DateFrom.ToUniversalTime(),
            command.DateTo.ToUniversalTime(),
            ct);

        var items = aggregate
            .Select(a => new GetReportResultData.GetReportItem(a.CategoryId, a.Amount))
            .ToArray();

        return Result<GetReportResultData>.Success(new GetReportResultData(items));
    }
}
