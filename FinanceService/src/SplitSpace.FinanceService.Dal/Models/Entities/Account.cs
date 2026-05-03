using SplitSpace.FinanceService.Common.Enums;

namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record Account
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal Balance { get; set; }
    public required AccountOwnerType OwnerType { get; init; }
    public required Guid OwnerId { get; init; }
}
