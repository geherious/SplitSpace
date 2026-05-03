using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services.Implementations;

public class SearchService : ISearchService
{
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;
    private readonly IExpenseRepository _expenseRepository;

    public SearchService(
        ISpaceMembershipRepository spaceMembershipRepository,
        IExpenseRepository expenseRepository)
    {
        _spaceMembershipRepository = spaceMembershipRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<Result<GetReportResultData>> GetReportAsync(GetReportCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<GetReportResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }
        
        var aggregate = await _expenseRepository.GetGroupedByCategory(command.SpaceId, command.DateFrom, command.DateTo);

        var items = aggregate.Select(a => new GetReportResultData.GetReportItem(a.CategoryId, a.Amount)).ToArray();
        return Result.Success(new GetReportResultData(items));
    }
}
