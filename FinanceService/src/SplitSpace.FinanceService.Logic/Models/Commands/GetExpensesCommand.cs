namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record GetExpensesCommand(Guid SpaceId, Guid UserId);
