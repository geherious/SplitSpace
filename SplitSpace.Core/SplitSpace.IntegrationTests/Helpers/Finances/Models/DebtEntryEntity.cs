namespace SplitSpace.IntegrationTests.Helpers.Finances.Models;

public sealed record DebtEntryEntity(
    Guid Id,
    Guid DebtId,
    Guid OwnedBy,
    Guid OwnedTo,
    decimal Total,
    string SourceType,
    Guid? ExpenseId,
    Guid? SplitId);
