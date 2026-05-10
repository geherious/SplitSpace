using SplitSpace.FinanceService.Api.FinanceService;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades;

public interface IFinanceServiceClientFacade
{
    Task<AddCategoryResponse> AddCategoryAsync(AddCategoryRequest request, CancellationToken cancellationToken);
    Task<GetCategoriesResponse> GetCategoriesAsync(GetCategoriesRequest request, CancellationToken cancellationToken);
    Task<AddTagResponse> AddTagAsync(AddTagRequest request, CancellationToken cancellationToken);
    Task<GetTagsResponse> GetTagsAsync(GetTagsRequest request, CancellationToken cancellationToken);
    Task<AddAccountResponse> AddAccountAsync(AddAccountRequest request, CancellationToken cancellationToken);
    Task<GetSpaceAccountsResponse> GetSpaceAccountsAsync(GetSpaceAccountsRequest request, CancellationToken cancellationToken);
    Task<GetPersonalAccountsResponse> GetPersonalAccountsAsync(GetPersonalAccountsRequest request, CancellationToken cancellationToken);
    Task<AddExpenseResponse> AddExpenseAsync(AddExpenseRequest request, CancellationToken cancellationToken);
    Task<GetExpensesResponse> GetExpensesAsync(GetExpensesRequest request, CancellationToken cancellationToken);
    Task<GetDebtsResponse> GetDebtsAsync(GetDebtsRequest request, CancellationToken cancellationToken);
    Task<SettleDebtResponse> SettleDebtAsync(SettleDebtRequest request, CancellationToken cancellationToken);
}
