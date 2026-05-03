using SplitSpace.FinanceService.Common.Enums;
using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services.Implementations;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseSplitRepository _expenseSplitRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IDebtRepository _debtRepository;

    public ExpenseService(ISpaceMembershipRepository spaceMembershipRepository,
        IUnitOfWork unitOfWork,
        IExpenseRepository expenseRepository,
        IExpenseSplitRepository expenseSplitRepository,
        ICategoryRepository categoryRepository,
        IAccountRepository accountRepository,
        IDebtRepository debtRepository)
    {
        _spaceMembershipRepository = spaceMembershipRepository;
        _unitOfWork = unitOfWork;
        _expenseRepository = expenseRepository;
        _expenseSplitRepository = expenseSplitRepository;
        _categoryRepository = categoryRepository;
        _accountRepository = accountRepository;
        _debtRepository = debtRepository;
    }

    public async Task<Result<AddExpenseResultData>> AddExpenseAsync(AddExpenseCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<AddExpenseResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }
        
        var category = await _categoryRepository.GetAsync(command.CategoryId);
        var account = await _accountRepository.GetAsync(command.AccountId);

        if (category is null)
        {
            return Result<AddExpenseResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Category not found"
            });
        }
        
        if (account is null ||
            (account.OwnerType == AccountOwnerType.Personal && account.OwnerId != command.UserId) ||
            (account.OwnerType == AccountOwnerType.Space && account.OwnerId != command.SpaceId))
        {
            return Result<AddExpenseResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Account not found"
            });
        }

        if (command.Split is not null &&
            account.OwnerType != AccountOwnerType.Personal)
        {
            return Result<AddExpenseResultData>.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = "Expense can be split only if paid by personal account"
            });
        }

        var expense = new Expense
        {
            Id = Guid.CreateVersion7(),
            SpaceId = command.SpaceId,
            CreatedBy = command.UserId,
            CategoryId = command.CategoryId,
            AccountId = command.AccountId,
            Amount = command.Amount,
            Description = command.Description,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        List<ExpenseSplit> splits = [];
        List<Debt> debts = [];

        switch (command.Split)
        {
            case null:
                break;
            case AddExpenseCommand.ExpenseSplit.Template
            {
                ExpenseSplitTemplate: AddExpenseCommand.ExpenseSplitTemplate.Equal
            }:
                (splits, debts) = await HandleEqualSplitAsync(expense);
                break;
            default:
                return Result<AddExpenseResultData>.Failure(new Error
                {
                    Type = ErrorType.FailedPrecondition,
                    Message = "Unsupported split type"
                });
        }

        await _expenseSplitRepository.AddAsync(splits);
        await _debtRepository.AddOrUpdateAsync(debts);
        await _expenseRepository.AddAsync(expense);
        await _accountRepository.UpdateAmountAsync(command.AccountId, -command.Amount);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(new AddExpenseResultData(expense.Id));
    }

    public async Task<Result<GetExpensesResultData>> GetExpensesAsync(GetExpensesCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<GetExpensesResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        var expenseAggregates = await _expenseRepository.GetBatchAsync(command.SpaceId);

        var result = new List<GetExpensesResultData.Expense>(capacity: expenseAggregates.Count);
        foreach (var aggregate in expenseAggregates)
        {
            Guid? accountId = aggregate.Account.OwnerType == AccountOwnerType.Personal
                ? aggregate.Account.OwnerId == command.UserId ? aggregate.Account.Id : null
                : aggregate.Account.Id;

            var splits = aggregate.Splits
                .Select(s => new GetExpensesResultData.ExpenseSplitItem
                {
                    UserId = s.UserId,
                    AmountToPay = s.AmountToPay,
                    ExpensePercent = Math.Round(s.AmountToPay / aggregate.Expense.Amount, 2)
                })
                .ToArray();

            GetExpensesResultData.ExpenseSplit? split = null;
            if (splits.Length > 0)
            {
                split = new GetExpensesResultData.ExpenseSplit { ExpenseSplitItems = splits };
            }
            
            result.Add(new GetExpensesResultData.Expense(
                aggregate.Expense.Id,
                aggregate.Expense.CategoryId,
                accountId,
                aggregate.Expense.Amount,
                aggregate.Expense.Description,
                split));
        }

        return Result.Success(new GetExpensesResultData(result));
    }

    private async Task<(List<ExpenseSplit> splits, List<Debt> debts)> HandleEqualSplitAsync(Expense expense)
    {
        var memberships = await _spaceMembershipRepository.GetBatchAsync(expense.SpaceId);
        var memberShare = expense.Amount / memberships.Count;

        var splits = new List<ExpenseSplit>(capacity: memberships.Count);
        var debts = new List<Debt>(capacity: memberships.Count);
        
        foreach (var member in memberships)
        {
            decimal toPay = memberShare;

            if (member.UserId == expense.CreatedBy)
            {
                toPay = 0;
            }

            var expenseSplit = new ExpenseSplit
            {
                Id = Guid.CreateVersion7(),
                ExpenseId = expense.Id,
                SpaceId = expense.SpaceId,
                UserId = member.UserId,
                AmountToPay = toPay,
            };
            
            Debt? debt = null;
            if (member.UserId != expense.CreatedBy)
            {
                var debtFrom = member.UserId;
                var debtTo = expense.CreatedBy;
                var debtAmount = memberShare;

                if (debtFrom > debtTo)
                {
                    (debtFrom, debtTo) = (debtTo, debtFrom);
                    debtAmount *= -1;
                }

                debt = new Debt
                {
                    Id = Guid.CreateVersion7(),
                    SpaceId = expense.SpaceId,
                    FromUserId = debtFrom,
                    ToUserId = debtTo,
                    Amount = debtAmount
                };
            }
            
            splits.Add(expenseSplit);
            
            if (debt != null)
                debts.Add(debt);
        }
        
        return (splits, debts);
    }
}
