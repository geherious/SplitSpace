using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Logic.Models.Results;

public record GetDebtsResultData(IReadOnlyCollection<Debt> Debts);
