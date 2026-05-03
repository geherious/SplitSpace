namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record GetSpaceAccountsCommand(Guid SpaceId, Guid UserId);
