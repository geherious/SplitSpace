using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class DebtDomainRepositoryTests : TestBase
{
    private readonly DebtTestRepository _sut;

    public DebtDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<DebtTestRepository>();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistDebtAndEntry()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var fromUser = UserId.New();
        var toUser = UserId.New();

        var debt = Debt.Create(spaceId, fromUser, toUser);
        var entry = DebtEntry.Create(debt.Id, fromUser, toUser, new Money(50m), new DebtEntry.DebtEntrySource.None());
        debt.ApplyEntry(entry);

        // Act
        await _sut.SaveAsync(debt, CancellationToken.None);
        var existing = await _sut.GetOrCreate(debt, CancellationToken.None);
        var entries = await _sut.ReadEntriesAsync(existing.Id);

        // Assert
        var persisted = entries.Should().ContainSingle().Subject;
        existing.Id.Should().Be(debt.Id);
        existing.SpaceId.Should().Be(spaceId);
        Math.Abs(existing.Total.Amount).Should().Be(50m);
        persisted.DebtId.Should().Be(debt.Id.Value);
        persisted.Total.Should().Be(50m);
        persisted.SourceType.Should().Be("none");
        persisted.ExpenseId.Should().BeNull();
        persisted.SplitId.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_WithExpenseSplitSource_ShouldPersistSplitLink()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var fromUser = UserId.New();
        var toUser = UserId.New();
        var expenseId = ExpenseId.New();
        var splitId = ExpenseSplitId.New();

        var debt = Debt.Create(spaceId, fromUser, toUser);
        var entry = DebtEntry.Create(
            debt.Id,
            fromUser,
            toUser,
            new Money(30m),
            new DebtEntry.DebtEntrySource.ExpenseSplit(expenseId, splitId));
        debt.ApplyEntry(entry);

        // Act
        await _sut.SaveAsync(debt, CancellationToken.None);
        var entries = await _sut.ReadEntriesAsync(debt.Id);

        // Assert
        var persisted = entries.Should().ContainSingle().Subject;
        persisted.SourceType.Should().Be("expense_split");
        persisted.ExpenseId.Should().Be(expenseId.Value);
        persisted.SplitId.Should().Be(splitId.Value);
    }

    [Fact]
    public async Task GetOrCreate_WhenDebtDoesNotExist_ShouldReturnGivenDebt()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var fromUser = UserId.New();
        var toUser = UserId.New();
        var debt = Debt.Create(spaceId, fromUser, toUser);

        // Act
        var result = await _sut.GetOrCreate(debt, CancellationToken.None);

        // Assert
        result.Id.Should().Be(debt.Id);
        result.Total.Amount.Should().Be(0m);
    }
}
