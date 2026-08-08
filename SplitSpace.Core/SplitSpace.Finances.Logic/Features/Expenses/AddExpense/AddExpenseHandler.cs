using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate.SplitPolicies;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Expenses.AddExpense;

public class AddExpenseHandler : ICommandHandler<AddExpenseCommand, Result<AddExpenseResultData>>
{
    private readonly IMediator _mediator;
    private readonly TimeProvider _timeProvider;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;
    private readonly ICategoryDomainRepository _categoryDomainRepository;
    private readonly IBalanceDomainRepository _balanceDomainRepository;
    private readonly IExpenseDomainRepository _expenseDomainRepository;

    public AddExpenseHandler(TimeProvider timeProvider, ISpacesServiceClientFacade spacesServiceClientFacade, ICategoryDomainRepository categoryDomainRepository, IBalanceDomainRepository balanceDomainRepository, IExpenseDomainRepository expenseDomainRepository, IMediator mediator)
    {
        _timeProvider = timeProvider;
        _spacesServiceClientFacade = spacesServiceClientFacade;
        _categoryDomainRepository = categoryDomainRepository;
        _balanceDomainRepository = balanceDomainRepository;
        _expenseDomainRepository = expenseDomainRepository;
        _mediator = mediator;
    }

    public async ValueTask<Result<AddExpenseResultData>> Handle(AddExpenseCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(command.UserId, ct);
        if (spaceIds.Contains(command.SpaceId) is false)
        {
            return Result<AddExpenseResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }
        
        var category = await _categoryDomainRepository.GetAsync(command.CategoryId, ct);
        if (category is null)
        {
            return Result<AddExpenseResultData>.Failure(new Error(ErrorType.NotFound, "Category not found"));
        }

        var balance = await _balanceDomainRepository.GetAsync(command.BalanceId, ct);
        if (balance is null ||
            balance.IsOwnedByUser(command.UserId) is false ||
            balance.IsOwnedBySpace(command.SpaceId) is false)
        {
            return Result<AddExpenseResultData>.Failure(new Error(ErrorType.NotFound, "Balance not found"));
        }

        if (command.Split is not ExpenseSplitMethod.None && balance.OwnerType != BalanceOwnerType.Personal)
        {
            return Result<AddExpenseResultData>.Failure(new Error(ErrorType.FailedPrecondition, "Expense can be split only if paid by personal balance"));
        }

        var currentTime = _timeProvider.GetUtcNow();
        var createdExpense = Expense.Create(
            command.SpaceId,
            command.UserId,
            command.CategoryId,
            balance.Id,
            new Money(command.Amount),
            command.Description,
            currentTime,
            GetPolicy(command.Split));

        if (createdExpense.IsSuccess is false)
        {
            return Result<AddExpenseResultData>.Failure(createdExpense.Errors);
        }

        var expense = createdExpense.ResultValue;
        var domainEvents = expense.DomainEvents.ToArray();

        await _expenseDomainRepository.SaveAsync(expense, ct);

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, ct);
        }

        return Result<AddExpenseResultData>.Success(new AddExpenseResultData(expense.Id));
    }

    private static IExpenseSplitPolicy GetPolicy(ExpenseSplitMethod splitMethod)
    {
        return splitMethod switch
        {
            ExpenseSplitMethod.None => new NoneSplitPolicy(),
            ExpenseSplitMethod.EqualSplit equalMethod => new EqualSplitPolicy(equalMethod),
            ExpenseSplitMethod.ExactAmount exactMethod => new ExactSplitPolicy(exactMethod),
            ExpenseSplitMethod.Percentages percentMethod => new PercentSplitPolicy(percentMethod),
            _ => throw new NotImplementedException()
        };
    }
}
