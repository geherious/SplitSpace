using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services;

public interface IDebtService
{
    Task<Result<GetDebtsResultData>> GetDebtsAsync(GetDebtsCommand command);
    Task<Result> SettleDebt(SettleDebtCommand command);
}
