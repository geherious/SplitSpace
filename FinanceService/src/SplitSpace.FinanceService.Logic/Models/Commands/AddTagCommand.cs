namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record AddTagCommand(Guid SpaceId, Guid UserId, string Name);
