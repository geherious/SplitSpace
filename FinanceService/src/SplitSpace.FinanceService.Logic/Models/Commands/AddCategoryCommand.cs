namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record AddCategoryCommand(Guid SpaceId, Guid UserId, string Name, Guid? ParentId, decimal? Limit);
