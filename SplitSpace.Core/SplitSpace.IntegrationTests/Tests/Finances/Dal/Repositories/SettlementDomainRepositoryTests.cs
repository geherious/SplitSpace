using AutoFixture;
using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class SettlementDomainRepositoryTests : TestBase
{
    private readonly SettlementTestRepository _sut;
    private readonly BalanceTestRepository _balances;
    private readonly Fixture _fixture = new();

    public SettlementDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<SettlementTestRepository>();
        _balances = GetRequiredService<BalanceTestRepository>();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistSettlement()
    {
        // Arrange
        var fromUser = UserId.New();
        var toUser = UserId.New();
        var fromBalance = Balance.CreatePersonal("My Wallet", fromUser, fromUser).ResultValue;
        fromBalance.Should().NotBeNull();
        await _balances.SaveAsync(fromBalance, CancellationToken.None);

        var createdAt = _fixture.Create<DateTimeOffset>().ToUniversalTime();
        var result = Settlement.Create(fromUser, fromBalance.Id, toUser, new Money(75m), createdAt);
        result.IsSuccess.Should().BeTrue();
        var settlement = result.ResultValue;
        settlement.Should().NotBeNull();

        // Act
        await _sut.SaveAsync(settlement, CancellationToken.None);
        var loaded = await _sut.GetAsync(settlement.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(settlement.Id.Value);
        loaded.FromUserId.Should().Be(fromUser.Value);
        loaded.FromBalanceId.Should().Be(fromBalance.Id.Value);
        loaded.ToUserId.Should().Be(toUser.Value);
        loaded.Amount.Should().Be(75m);
        loaded.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task GetAsync_WhenSettlementDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var settlementId = SettlementId.New();

        // Act
        var loaded = await _sut.GetAsync(settlementId, CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }
}
