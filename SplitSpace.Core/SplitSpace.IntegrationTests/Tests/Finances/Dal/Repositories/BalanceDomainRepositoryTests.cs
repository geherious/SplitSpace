using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class BalanceDomainRepositoryTests : TestBase
{
    private readonly BalanceTestRepository _sut;

    public BalanceDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<BalanceTestRepository>();
    }

    [Fact]
    public async Task CreatePersonal_ShouldPersistBalance()
    {
        // Arrange
        var ownerId = UserId.New();
        var result = Balance.CreatePersonal("My Wallet", ownerId, ownerId);
        result.IsSuccess.Should().BeTrue();
        var balance = result.ResultValue;
        balance.Should().NotBeNull();

        // Act
        await _sut.SaveAsync(balance, CancellationToken.None);
        var loaded = await _sut.GetAsync(balance.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(balance.Id);
        loaded.Name.Should().Be("My Wallet");
        loaded.Total.Amount.Should().Be(0m);
        loaded.OwnerType.Should().Be(BalanceOwnerType.Personal);
        loaded.OwnerId.Should().Be(ownerId.Value);
        loaded.CreatedBy.Should().Be(ownerId.Value);
    }

    [Fact]
    public async Task CreateSpace_ShouldPersistBalance()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var creator = UserId.New();
        var balance = Balance.CreateSpace("Trip", spaceId, creator);

        // Act
        await _sut.SaveAsync(balance, CancellationToken.None);
        var loaded = await _sut.GetAsync(balance.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(balance.Id);
        loaded.Name.Should().Be("Trip");
        loaded.OwnerType.Should().Be(BalanceOwnerType.Space);
        loaded.OwnerId.Should().Be(spaceId.Value);
    }

    [Fact]
    public async Task GetAsync_WhenBalanceDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var balanceId = BalanceId.New();

        // Act
        var loaded = await _sut.GetAsync(balanceId, CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task ApplyEntry_WithNoneSource_ShouldPersistEntryAndUpdateTotal()
    {
        // Arrange
        var ownerId = UserId.New();
        var balance = Balance.CreatePersonal("My Wallet", ownerId, ownerId).ResultValue;
        balance.Should().NotBeNull();
        await _sut.SaveAsync(balance, CancellationToken.None);

        var entry = BalanceEntry.Create(balance.Id, new Money(100m), new BalanceEntry.BalanceEntrySource.None());
        balance.ApplyEntry(entry);

        // Act
        await _sut.SaveAsync(balance, CancellationToken.None);
        var loaded = await _sut.GetAsync(balance.Id, CancellationToken.None);
        var entries = await _sut.ReadEntriesAsync(balance.Id);

        // Assert
        loaded.Should().NotBeNull();
        var persisted = entries.Should().ContainSingle().Subject;
        loaded.Total.Amount.Should().Be(100m);
        persisted.BalanceId.Should().Be(balance.Id.Value);
        persisted.Total.Should().Be(100m);
        persisted.SourceType.Should().Be("none");
        persisted.ExpenseId.Should().BeNull();
    }

    [Fact]
    public async Task ApplyEntry_WithExpenseSource_ShouldPersistEntryWithExpenseId()
    {
        // Arrange
        var ownerId = UserId.New();
        var balance = Balance.CreatePersonal("My Wallet", ownerId, ownerId).ResultValue;
        balance.Should().NotBeNull();
        var expenseId = ExpenseId.New();
        await _sut.SaveAsync(balance, CancellationToken.None);

        var entry = BalanceEntry.Create(balance.Id, new Money(250m), new BalanceEntry.BalanceEntrySource.Expense(expenseId));
        balance.ApplyEntry(entry);

        // Act
        await _sut.SaveAsync(balance, CancellationToken.None);
        var entries = await _sut.ReadEntriesAsync(balance.Id);

        // Assert
        var persisted = entries.Should().ContainSingle().Subject;
        persisted.SourceType.Should().Be("expense");
        persisted.ExpenseId.Should().Be(expenseId.Value);
    }
}
