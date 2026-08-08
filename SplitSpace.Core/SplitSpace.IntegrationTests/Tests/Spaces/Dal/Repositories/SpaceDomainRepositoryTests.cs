using FluentAssertions;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Spaces.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Spaces.Dal.Repositories;

public sealed class SpaceDomainRepositoryTests : TestBase
{
    private readonly SpaceTestRepository _sut;

    public SpaceDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<SpaceTestRepository>();
    }

    [Fact]
    public async Task CreatePrivate_ShouldPersistSpaceAndOwnerMember()
    {
        // Arrange
        var ownerId = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var space = Space.CreatePrivate("My Space", ownerId, createdAt);

        // Act
        await _sut.SaveAsync(space, CancellationToken.None);
        var loaded = await _sut.GetAsync(space.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(space.Id);
        loaded.Name.Should().Be("My Space");
        loaded.Type.Should().Be(SpaceType.Private);
        loaded.OwnerId.Should().Be(ownerId);
        loaded.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromMilliseconds(1));
        var ownerMember = loaded.Members.Should().ContainSingle().Subject;
        ownerMember.UserId.Should().Be(ownerId);
        ownerMember.Role.Should().Be(SpaceMemberRole.Owner);
        ownerMember.JoinedAt.Should().BeCloseTo(createdAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task CreateShared_ShouldPersistSpaceAndAllMembers()
    {
        // Arrange
        var ownerId = UserId.New();
        var memberOneId = UserId.New();
        var memberTwoId = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var space = Space.CreateShared("Trip", ownerId, [memberOneId, memberTwoId], createdAt);

        // Act
        await _sut.SaveAsync(space, CancellationToken.None);
        var loaded = await _sut.GetAsync(space.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Type.Should().Be(SpaceType.Shared);
        loaded.OwnerId.Should().Be(ownerId);
        loaded.Members.Should().HaveCount(3);
        var ownerMember = loaded.Members.Should().ContainSingle(m => m.UserId == ownerId).Subject;
        ownerMember.Role.Should().Be(SpaceMemberRole.Owner);
        var otherMembers = loaded.Members.Where(m => m.UserId != ownerId).ToList();
        otherMembers.Should().HaveCount(2);
        otherMembers.Should().OnlyContain(m => m.Role == SpaceMemberRole.Member);
        otherMembers.Select(m => m.UserId).Should().BeEquivalentTo(new[] { memberOneId, memberTwoId });
    }

    [Fact]
    public async Task GetAsync_WhenSpaceDoesNotExist_ShouldReturnNull()
    {
        // Act
        var loaded = await _sut.GetAsync(SpaceId.New(), CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task AddMember_ShouldPersistNewMember()
    {
        // Arrange
        var ownerId = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var space = Space.CreatePrivate("My Space", ownerId, createdAt);
        await _sut.SaveAsync(space, CancellationToken.None);

        var newMemberId = UserId.New();
        var joinedAt = createdAt.AddDays(1);
        var added = space.AddMember(newMemberId, joinedAt);
        added.Should().BeTrue();

        // Act
        await _sut.SaveAsync(space, CancellationToken.None);
        var loaded = await _sut.GetAsync(space.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Members.Should().HaveCount(2);
        var member = loaded.Members.Should().ContainSingle(m => m.UserId == newMemberId).Subject;
        member.Role.Should().Be(SpaceMemberRole.Member);
        member.JoinedAt.Should().BeCloseTo(joinedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task DeleteByOwner_ShouldDeleteSpace()
    {
        // Arrange
        var ownerId = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var space = Space.CreatePrivate("My Space", ownerId, createdAt);
        await _sut.SaveAsync(space, CancellationToken.None);

        var deleted = space.DeleteBy(ownerId);
        deleted.Should().BeTrue();

        // Act
        await _sut.SaveAsync(space, CancellationToken.None);
        var loaded = await _sut.GetAsync(space.Id, CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByNonOwner_ShouldNotDeleteSpace()
    {
        // Arrange
        var ownerId = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var space = Space.CreatePrivate("My Space", ownerId, createdAt);
        await _sut.SaveAsync(space, CancellationToken.None);

        var deleted = space.DeleteBy(UserId.New());
        deleted.Should().BeFalse();

        // Act
        await _sut.SaveAsync(space, CancellationToken.None);
        var loaded = await _sut.GetAsync(space.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(space.Id);
    }
}
