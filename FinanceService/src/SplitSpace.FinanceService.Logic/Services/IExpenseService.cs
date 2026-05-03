using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services;

public interface IExpenseService
{
    Task<Result<AddExpenseResultData>> AddExpenseAsync(AddExpenseCommand command);
    
    Task<Result<GetExpensesResultData>> GetExpensesAsync(GetExpensesCommand command);
}
