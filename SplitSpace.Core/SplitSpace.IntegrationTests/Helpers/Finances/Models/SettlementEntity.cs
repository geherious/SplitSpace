namespace SplitSpace.IntegrationTests.Helpers.Finances.Models;

public sealed record SettlementEntity(
    Guid Id,
    Guid FromUserId,
    Guid FromBalanceId,
    Guid ToUserId,
    decimal Amount,
    DateTimeOffset CreatedAt);