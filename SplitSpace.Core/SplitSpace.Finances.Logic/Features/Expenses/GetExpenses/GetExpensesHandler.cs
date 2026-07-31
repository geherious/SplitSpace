using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Expenses.GetExpenses;

public class GetExpensesHandler : IQueryHandler<GetExpensesCommand, Result<GetExpensesResultData>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public GetExpensesHandler(IExpenseRepository expenseRepository, ISpacesServiceClientFacade spacesServiceClientFacade)
    {
        _expenseRepository = expenseRepository;
        _spacesServiceClientFacade = spacesServiceClientFacade;
    }

    public async ValueTask<Result<GetExpensesResultData>> Handle(GetExpensesCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(new UserId(command.UserId), ct);
        if (spaceIds.Contains(new SpaceId(command.SpaceId)) is false)
        {
            return Result<GetExpensesResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        var expenseAggregates = await _expenseRepository.GetBatchAsync(command.SpaceId, ct);

        var result = new List<GetExpensesResultData.Expense>(capacity: expenseAggregates.Count);

        foreach (var aggregate in expenseAggregates)
        {
            Guid? balanceId = aggregate.BalanceEntity.OwnerType == BalanceOwnerType.Personal
                ? aggregate.BalanceEntity.OwnerId == command.UserId ? aggregate.BalanceEntity.Id : null
                : aggregate.BalanceEntity.Id;

            var splitItems = aggregate.Splits
                .Select(s => new GetExpensesResultData.ExpenseSplitItem
                {
                    UserId = s.UserId,
                    AmountToPay = s.AmountToPay,
                    ExpensePercent = Math.Round(s.AmountToPay / aggregate.Expense.Amount, 2)
                })
                .ToArray();

            GetExpensesResultData.ExpenseSplit? split = null;
            if (splitItems.Length > 0)
            {
                split = new GetExpensesResultData.ExpenseSplit { ExpenseSplitItems = splitItems };
            }

            result.Add(new GetExpensesResultData.Expense(
                aggregate.Expense.Id,
                aggregate.Expense.CategoryId,
                balanceId,
                aggregate.Expense.Amount,
                aggregate.Expense.Description,
                split));
        }

        return Result<GetExpensesResultData>.Success(new GetExpensesResultData(result));
    }
}
