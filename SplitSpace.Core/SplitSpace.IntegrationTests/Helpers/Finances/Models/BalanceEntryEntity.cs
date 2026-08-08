namespace SplitSpace.IntegrationTests.Helpers.Finances.Models;

public sealed record BalanceEntryEntity(
    Guid Id,
    Guid BalanceId,
    decimal Total,
    string SourceType,
    Guid? ExpenseId);
