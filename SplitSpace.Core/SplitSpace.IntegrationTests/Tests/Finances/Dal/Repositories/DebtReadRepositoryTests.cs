using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class DebtReadRepositoryTests : TestBase
{
    private readonly DebtTestRepository _sut;

    public DebtReadRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<DebtTestRepository>();
    }

    [Fact]
    public async Task GetBatchAsync_ShouldReturnDebtsForSpaceAndUser()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var fromUser = UserId.New();
        var toUser = UserId.New();
        var unrelatedUser = UserId.New();

        var debt = Debt.Create(spaceId, fromUser, toUser);
        var entry = DebtEntry.Create(debt.Id, fromUser, toUser, new Money(20m), new DebtEntry.DebtEntrySource.None());
        debt.ApplyEntry(entry);
        await _sut.SaveAsync(debt, CancellationToken.None);

        // Act
        var forFromUser = await _sut.GetBatchAsync(spaceId.Value, fromUser.Value, CancellationToken.None);
        var forToUser = await _sut.GetBatchAsync(spaceId.Value, toUser.Value, CancellationToken.None);
        var forUnrelated = await _sut.GetBatchAsync(spaceId.Value, unrelatedUser.Value, CancellationToken.None);

        // Assert
        var persisted = forFromUser.Should().ContainSingle().Subject;
        persisted.Id.Should().Be(debt.Id.Value);
        persisted.SpaceId.Should().Be(spaceId.Value);
        Math.Abs(persisted.Amount).Should().Be(20m);
        forToUser.Should().ContainSingle();
        forUnrelated.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBatchAsync_WhenNoDebts_ShouldReturnEmpty()
    {
        // Arrange
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.GetBatchAsync(spaceId, userId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
