using System.ComponentModel.DataAnnotations;
using Grpc.Core;
using SplitSpace.FinanceService.Api.FinanceService;
using SplitSpace.FinanceService.Helpers;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Services;
using SplitSpace.FinanceService.Mappers;

namespace SplitSpace.FinanceService.Services;

public class FinanceServiceGrpc : Api.FinanceService.FinanceService.FinanceServiceBase
{
    private readonly ICategoryService _categoryService;
    private readonly IAccountService _accountService;
    private readonly ITagService _tagService;
    private readonly IExpenseService _expenseService;
    private readonly IDebtService _debtService;

    public FinanceServiceGrpc(ICategoryService categoryService,
        IAccountService accountService,
        ITagService tagService,
        IExpenseService expenseService,
        IDebtService debtService)
    {
        _categoryService = categoryService;
        _accountService = accountService;
        _tagService = tagService;
        _expenseService = expenseService;
        _debtService = debtService;
    }

    public override async Task<AddCategoryResponse> AddCategory(AddCategoryRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var parentCategoryId = request.ParentCategoryId.ToNullableGuidOrThrow(nameof(request.ParentCategoryId));
        var limit = request.Limit.ToNullableDecimalOrThrow(nameof(request.Limit));

        var result = await _categoryService.AddCategoryAsync(new AddCategoryCommand(
            spaceId,
            userId,
            request.Name,
            parentCategoryId,
            limit));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new AddCategoryResponse
        {
            CategoryId = result.Value.CategoryId.ToString()
        };
    }

    public override async Task<GetCategoriesResponse> GetCategories(GetCategoriesRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _categoryService.GetCategoriesAsync(new GetCategoriesCommand(spaceId, userId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetCategoriesResponse
        {
            Categories = { result.Value.Categories.Select(c => new GetCategoriesResponse.Types.Category
            {
                CategoryId = c.Id.ToString(),
                Name = c.Name,
                ParentCategoryId = c.ParentId?.ToString(),
                Limit = c.Limit.ToNullableMoney(),
            }) }
        };
    }

    public override async Task<AddAccountResponse> AddAccount(AddAccountRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var balance = request.Balance.ToDecimalOrThrow(nameof(request.Balance));

        var result = await _accountService.AddAccountAsync(new AddAccountCommand(
            spaceId,
            userId,
            request.Name,
            balance,
            request.OwnerType.ToDomain()));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new AddAccountResponse
        {
            AccountId = result.Value.AccountId.ToString()
        };
    }

    public override async Task<GetSpaceAccountsResponse> GetSpaceAccounts(GetSpaceAccountsRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _accountService.GetSpaceAccountsAsync(new GetSpaceAccountsCommand(spaceId, userId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetSpaceAccountsResponse
        {
            Accounts = { result.Value.Accounts.Select(a => new GetSpaceAccountsResponse.Types.Account
            {
                AccountId = a.Id.ToString(),
                Name = a.Name,
                Balance = a.Balance.ToMoney(),
                SpaceId = a.SpaceId.ToString()
            }) }
        };
    }

    public override async Task<GetPersonalAccountsResponse> GetPersonalAccounts(GetPersonalAccountsRequest request, ServerCallContext context)
    {
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _accountService.GetPersonalAccountsAsync(new GetPersonalAccountsCommand(userId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetPersonalAccountsResponse
        {
            Accounts = { result.Value.Accounts.Select(a => new GetPersonalAccountsResponse.Types.Account
            {
                AccountId = a.Id.ToString(),
                Name = a.Name,
                Balance = a.Balance.ToMoney(),
                UserId =  a.UserId.ToString()
            }) }
        };
    }

    public override async Task<AddTagResponse> AddTag(AddTagRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _tagService.AddTagAsync(new AddTagCommand(spaceId, userId, request.Name));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new AddTagResponse
        {
            TagId = result.Value.TagId.ToString()
        };
    }

    public override async Task<GetTagsResponse> GetTags(GetTagsRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _tagService.GetTagsAsync(new GetTagsCommand(spaceId, userId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetTagsResponse
        {
            Tags = { result.Value.Tags.Select(t => new GetTagsResponse.Types.Tag
            {
                TagId = t.Id.ToString(),
                Name = t.Name
            }) }
        };
    }

    public override async Task<AddExpenseResponse> AddExpense(AddExpenseRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));
        var categoryId = request.CategoryId.ToGuidOrThrow(nameof(request.CategoryId));
        var accountId = request.AccountId.ToGuidOrThrow(nameof(request.AccountId));
        var amount = request.Amount.ToDecimalOrThrow(nameof(request.Amount));

        var result = await _expenseService.AddExpenseAsync(new AddExpenseCommand(
            spaceId,
            userId,
            categoryId,
            accountId,
            amount,
            request.Description,
            request.Split is null
                ? null
                : request.Split.TypeCase switch
                {
                    AddExpenseRequest.Types.Split.TypeOneofCase.None => null,
                    AddExpenseRequest.Types.Split.TypeOneofCase.TemplateSplit =>
                        request.Split.TemplateSplit.ToCommand(),
                    _ => throw new ValidationException("Invalid split type")
                }));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new AddExpenseResponse
        {
            ExpenseId = result.Value.ExpenseId.ToString()
        };
    }

    public override async Task<GetExpensesResponse> GetExpenses(GetExpensesRequest request, ServerCallContext context)
    {
        var spaceId = request.SpaceId.ToGuidOrThrow(nameof(request.SpaceId));
        var userId = request.UserId.ToGuidOrThrow(nameof(request.UserId));

        var result = await _expenseService.GetExpensesAsync(new GetExpensesCommand(spaceId, userId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetExpensesResponse
        {
            Expenses = { result.Value.Expenses.Select(e => new GetExpensesResponse.Types.Expense
            {
                ExpenseId = e.ExpenseId.ToString(),
                CategoryId = e.CategoryId.ToString(),
                AccountId = e.AccountId.ToString(),
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

        var result = await _debtService.GetDebtsAsync(new GetDebtsCommand(spaceId, userId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new GetDebtsResponse
        {
            Debts = { result.Value.Debts.Select(d => new GetDebtsResponse.Types.Debt
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
        var accountId = request.AccountId.ToGuidOrThrow(nameof(request.AccountId));
        var amount = request.Amount.ToDecimalOrThrow(nameof(request.Amount));

        var result = await _debtService.SettleDebt(new SettleDebtCommand(spaceId, userId, accountId, amount, userToId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors.First());
        }

        return new SettleDebtResponse();
    }
}
