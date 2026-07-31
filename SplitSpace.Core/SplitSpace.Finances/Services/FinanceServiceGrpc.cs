using Grpc.Core;
using Mediator;
using SplitSpace.Finances.Api.FinanceService;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Helpers;
using SplitSpace.Finances.Logic.Features.Balances.AddBalance;
using SplitSpace.Finances.Logic.Features.Balances.GetPersonalBalances;
using SplitSpace.Finances.Logic.Features.Balances.GetSpaceBalances;
using SplitSpace.Finances.Logic.Features.Categories.AddCategory;
using SplitSpace.Finances.Logic.Features.Categories.GetCategories;
using SplitSpace.Finances.Logic.Features.Debts.GetDebts;
using SplitSpace.Finances.Logic.Features.Expenses.AddExpense;
using SplitSpace.Finances.Logic.Features.Expenses.GetExpenses;
using SplitSpace.Finances.Logic.Features.Settlements.CreateSettlement;
using SplitSpace.Finances.Mappers;

namespace SplitSpace.Finances.Services;

public class FinanceServiceGrpc : FinanceService.FinanceServiceBase
{
    private readonly IMediator _mediator;

    public FinanceServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<AddCategoryResponse> AddCategory(AddCategoryRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var parentCategoryId = request.ParentCategoryId.ToNullableGuidOrThrow(nameof(request.ParentCategoryId));
        var limit = request.Limit.ToNullableDecimalOrThrow(nameof(request.Limit));

        var result = await _mediator.Send(new AddCategoryCommand(
            new SpaceId(spaceId),
            new UserId(userId),
            request.Name,
            parentCategoryId is null ? null : new CategoryId(parentCategoryId.Value),
            limit), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new AddCategoryResponse
        {
            CategoryId = result.ResultValue.CategoryId.ToString()
        };
    }

    public override async Task<GetCategoriesResponse> GetCategories(GetCategoriesRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _mediator.Send(new GetCategoriesCommand(new SpaceId(spaceId), new UserId(userId)), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetCategoriesResponse
        {
            Categories = { result.ResultValue.Categories.Select(c => new GetCategoriesResponse.Types.Category
            {
                CategoryId = c.Id.ToString(),
                Name = c.Name,
                ParentCategoryId = c.ParentId?.ToString(),
                Limit = c.Limit.ToNullableMoney(),
            }) }
        };
    }

    public override async Task<AddBalanceResponse> AddBalance(AddBalanceRequest request, ServerCallContext context)
    {
        var userId = request.ActorUserId.ToGuidOrThrow(nameof(request.ActorUserId));
        var balance = request.Balance.ToDecimalOrThrow(nameof(request.Balance));

        Guid ownerId;
        BalanceOwnerType ownerType;
        switch (request.OwnerCase)
        {
            case AddBalanceRequest.OwnerOneofCase.OwnerSpaceId:
                ownerId = request.OwnerSpaceId.ToGuidOrThrow(nameof(request.OwnerSpaceId));
                ownerType = BalanceOwnerType.Space;
                break;
            case AddBalanceRequest.OwnerOneofCase.OwnerUserId:
                ownerId = request.OwnerUserId.ToGuidOrThrow(nameof(request.OwnerUserId));
                ownerType = BalanceOwnerType.Personal;
                break;
            case AddBalanceRequest.OwnerOneofCase.None:
            default:
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid owner oneof case"));
        }

        var result = await _mediator.Send(new AddBalanceCommand(
            new UserId(userId),
            ownerId,
            ownerType,
            request.Name,
            balance), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new AddBalanceResponse
        {
            BalanceId = result.ResultValue.BalanceId.ToString()
        };
    }

    public override async Task<GetSpaceBalancesResponse> GetSpaceBalances(GetSpaceBalancesRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _mediator.Send(new GetSpaceBalancesCommand(new SpaceId(spaceId), new UserId(userId)), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetSpaceBalancesResponse
        {
            Balances = { result.ResultValue.Accounts.Select(a => new GetSpaceBalancesResponse.Types.Balance
            {
                BalanceId = a.Id.ToString(),
                Name = a.Name,
                Total = a.Total.ToMoney(),
                SpaceId = a.SpaceId.ToString()
            }) }
        };
    }

    public override async Task<GetPersonalBalancesResponse> GetPersonalBalances(GetPersonalBalancesRequest request, ServerCallContext context)
    {
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _mediator.Send(new GetPersonalBalancesCommand(userId), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetPersonalBalancesResponse
        {
            Balances = { result.ResultValue.Accounts.Select(a => new GetPersonalBalancesResponse.Types.Balance
            {
                BalanceId = a.Id.ToString(),
                Name = a.Name,
                Total = a.Balance.ToMoney(),
                UserId = a.UserId.ToString()
            }) }
        };
    }

    public override async Task<AddExpenseResponse> AddExpense(AddExpenseRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var categoryId = request.CategoryId.ToGuidOrThrow(nameof(request.CategoryId));
        var accountId = request.BalanceId.ToGuidOrThrow(nameof(request.BalanceId));
        var amount = request.Amount.ToDecimalOrThrow(nameof(request.Amount));
        var createdAt = request.CreatedAt.ToDateTimeOffsetOrThrow(nameof(request.CreatedAt));

        var split = request.Split?.ToCommand() ?? new ExpenseSplitMethod.None();

        var result = await _mediator.Send(new AddExpenseCommand(
            new SpaceId(spaceId),
            new UserId(userId),
            new CategoryId(categoryId),
            new BalanceId(accountId),
            amount,
            request.Description,
            split,
            createdAt), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new AddExpenseResponse
        {
            ExpenseId = result.ResultValue.ExpenseId.ToString()
        };
    }

    public override async Task<GetExpensesResponse> GetExpenses(GetExpensesRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _mediator.Send(new GetExpensesCommand(spaceId, userId), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetExpensesResponse
        {
            Expenses = { result.ResultValue.Expenses.Select(e => new GetExpensesResponse.Types.Expense
            {
                ExpenseId = e.ExpenseId.ToString(),
                CategoryId = e.CategoryId.ToString(),
                BalanceId = e.BalanceId.ToString(),
                Amount = e.Amount.ToMoney(),
                Description = e.Description,
                Split = e.Split is null
                ? null
                : new GetExpensesResponse.Types.ExpenseSplit
                {
                    SplitItems = { e.Split.ExpenseSplitItems.Select(si => new GetExpensesResponse.Types.ExpenseSplitItem
                    {
                        UserId = si.UserId.ToString(),
                        AmountToPay = si.AmountToPay.ToMoney(),
                    }) }
                }
            }) }
        };
    }

    public override async Task<GetDebtsResponse> GetDebts(GetDebtsRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _mediator.Send(new GetDebtsCommand(spaceId, userId), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetDebtsResponse
        {
            Debts = { result.ResultValue.Debts.Select(d => new GetDebtsResponse.Types.Debt
            {
                DebtId = d.Id.ToString(),
                SpaceId = d.SpaceId.ToString(),
                FromUserId = d.FromUserId.ToString(),
                ToUserId = d.ToUserId.ToString(),
                Amount = d.Amount.ToMoney(),
            }) }
        };
    }

    public override async Task<SettleDebtResponse> SettleDebt(SettleDebtRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var userToId = request.UserTo.ToGuidOrThrow(nameof(request.UserTo));
        var accountId = request.BalanceId.ToGuidOrThrow(nameof(request.BalanceId));
        var amount = request.Amount.ToDecimalOrThrow(nameof(request.Amount));

        var result = await _mediator.Send(new CreateSettlementCommand(spaceId, userId, accountId, amount, userToId), context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new SettleDebtResponse();
    }
}
