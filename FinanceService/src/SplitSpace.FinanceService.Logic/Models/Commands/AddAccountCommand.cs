using SplitSpace.FinanceService.Common.Enums;

namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record AddAccountCommand(
    Guid SpaceId,
    Guid UserId,
    string Name,
    decimal Balance,
    AccountOwnerType AccountOwnerType);
