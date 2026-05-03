using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services;

public interface IAccountService
{
    Task<Result<AddAccountResultData>> AddAccountAsync(AddAccountCommand command);
    
    Task<Result<GetSpaceAccountsResultData>> GetSpaceAccountsAsync(GetSpaceAccountsCommand command);
    
    Task<Result<GetPersonalAccountsResultData>> GetPersonalAccountsAsync(GetPersonalAccountsCommand command);
}
