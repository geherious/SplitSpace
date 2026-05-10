using SplitSpace.FinanceService.Api.FinanceService;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades.Implementations;

public class FinanceServiceClientFacade : IFinanceServiceClientFacade
{
    private readonly FinanceService.Api.FinanceService.FinanceService.FinanceServiceClient _financeServiceClient;

    public FinanceServiceClientFacade(FinanceService.Api.FinanceService.FinanceService.FinanceServiceClient financeServiceClient)
    {
        _financeServiceClient = financeServiceClient;
    }

    public async Task<AddCategoryResponse> AddCategoryAsync(AddCategoryRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.AddCategoryAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<GetCategoriesResponse> GetCategoriesAsync(GetCategoriesRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.GetCategoriesAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<AddTagResponse> AddTagAsync(AddTagRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.AddTagAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<GetTagsResponse> GetTagsAsync(GetTagsRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.GetTagsAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<AddAccountResponse> AddAccountAsync(AddAccountRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.AddAccountAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<GetSpaceAccountsResponse> GetSpaceAccountsAsync(GetSpaceAccountsRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.GetSpaceAccountsAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<GetPersonalAccountsResponse> GetPersonalAccountsAsync(GetPersonalAccountsRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.GetPersonalAccountsAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<AddExpenseResponse> AddExpenseAsync(AddExpenseRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.AddExpenseAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<GetExpensesResponse> GetExpensesAsync(GetExpensesRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.GetExpensesAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<GetDebtsResponse> GetDebtsAsync(GetDebtsRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.GetDebtsAsync(request, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<SettleDebtResponse> SettleDebtAsync(SettleDebtRequest request, CancellationToken cancellationToken)
    {
        var response = await _financeServiceClient.SettleDebtAsync(request, cancellationToken: cancellationToken);
        return response;
    }
}
