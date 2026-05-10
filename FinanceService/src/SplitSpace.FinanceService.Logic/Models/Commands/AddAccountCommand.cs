using SplitSpace.FinanceService.Common.Enums;

namespace SplitSpace.FinanceService.Logic.Models.Commands;

public record AddAccountCommand(
    Guid UserId,
    Guid OwnerId,
    AccountOwnerType AccountOwnerType,
    string Name,
    decimal Balance);
