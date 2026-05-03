namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record GetReportCommand(Guid SpaceId, Guid UserId, DateTimeOffset DateFrom, DateTimeOffset DateTo);
