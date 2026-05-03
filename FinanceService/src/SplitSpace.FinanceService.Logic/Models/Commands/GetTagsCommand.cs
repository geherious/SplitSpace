namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record GetTagsCommand(Guid SpaceId, Guid UserId);
