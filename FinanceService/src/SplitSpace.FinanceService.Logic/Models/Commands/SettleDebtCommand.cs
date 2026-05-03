namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record SettleDebtCommand(Guid SpaceId, Guid UserId, Guid AccountId, decimal Amount, Guid ToUserId);
