namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record GetDebtsCommand(Guid SpaceId, Guid UserId);
