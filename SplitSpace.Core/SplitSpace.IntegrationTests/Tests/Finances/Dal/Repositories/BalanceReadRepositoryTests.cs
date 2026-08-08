using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class BalanceReadRepositoryTests : TestBase
{
    private readonly BalanceTestRepository _sut;

    public BalanceReadRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<BalanceTestRepository>();
    }

    [Fact]
    public async Task GetSpaceBalanceBatchAsync_ShouldReturnOnlySpaceBalances()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var ownerId = UserId.New();

        var spaceBalance = Balance.CreateSpace("Trip", spaceId, ownerId);
        await _sut.SaveAsync(spaceBalance, CancellationToken.None);

        var personalBalance = Balance.CreatePersonal("My Wallet", ownerId, ownerId).ResultValue;
        personalBalance.Should().NotBeNull();
        await _sut.SaveAsync(personalBalance, CancellationToken.None);

        // Act
        var result = await _sut.GetSpaceBalanceBatchAsync(spaceId, CancellationToken.None);

        // Assert
        var persisted = result.Should().ContainSingle().Subject;
        persisted.Id.Should().Be(spaceBalance.Id.Value);
        persisted.Name.Should().Be("Trip");
        persisted.OwnerType.Should().Be(BalanceOwnerType.Space);
        persisted.OwnerId.Should().Be(spaceId.Value);
    }

    [Fact]
    public async Task GetUserBalanceBatchAsync_ShouldReturnOnlyPersonalBalances()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var ownerId = UserId.New();

        var spaceBalance = Balance.CreateSpace("Trip", spaceId, ownerId);
        await _sut.SaveAsync(spaceBalance, CancellationToken.None);

        var personalBalance = Balance.CreatePersonal("My Wallet", ownerId, ownerId).ResultValue;
        personalBalance.Should().NotBeNull();
        await _sut.SaveAsync(personalBalance, CancellationToken.None);

        // Act
        var result = await _sut.GetUserBalanceBatchAsync(ownerId, CancellationToken.None);

        // Assert
        var persisted = result.Should().ContainSingle().Subject;
        persisted.Id.Should().Be(personalBalance.Id.Value);
        persisted.OwnerType.Should().Be(BalanceOwnerType.Personal);
        persisted.OwnerId.Should().Be(ownerId.Value);
    }

    [Fact]
    public async Task GetSpaceBalanceBatchAsync_WhenNoBalances_ShouldReturnEmpty()
    {
        // Arrange
        var spaceId = SpaceId.New();

        // Act
        var result = await _sut.GetSpaceBalanceBatchAsync(spaceId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
