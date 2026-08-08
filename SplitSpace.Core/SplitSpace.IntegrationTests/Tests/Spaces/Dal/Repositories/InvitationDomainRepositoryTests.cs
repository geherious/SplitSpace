using FluentAssertions;
using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Spaces.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Spaces.Dal.Repositories;

public sealed class InvitationDomainRepositoryTests : TestBase
{
    private readonly InvitationTestRepository _sut;
    private readonly SpaceTestRepository _spaceRepository;

    public InvitationDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<InvitationTestRepository>();
        _spaceRepository = GetRequiredService<SpaceTestRepository>();
    }

    [Fact]
    public async Task Create_ShouldPersistInvitation()
    {
        // Arrange
        var spaceId = await CreateSpaceAsync();
        var invitedUserId = UserId.New();
        var invitedBy = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var invitation = Invitation.Create(spaceId, invitedUserId, invitedBy, createdAt);

        // Act
        await _sut.SaveAsync(invitation, CancellationToken.None);
        var loaded = await _sut.GetAsync(invitation.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(invitation.Id);
        loaded.SpaceId.Should().Be(spaceId);
        loaded.InvitedUserId.Should().Be(invitedUserId);
        loaded.InvitedBy.Should().Be(invitedBy);
        loaded.Status.Should().Be(InvitationStatus.Created);
        loaded.ExpiresAt.Should().BeCloseTo(createdAt.AddDays(30), TimeSpan.FromMilliseconds(1));
        loaded.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task GetAsync_WhenInvitationDoesNotExist_ShouldReturnNull()
    {
        // Act
        var loaded = await _sut.GetAsync(InvitationId.New(), CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task AcceptByUser_ShouldPersistAcceptedStatus()
    {
        // Arrange
        var spaceId = await CreateSpaceAsync();
        var invitedUserId = UserId.New();
        var invitedBy = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var invitation = Invitation.Create(spaceId, invitedUserId, invitedBy, createdAt);
        await _sut.SaveAsync(invitation, CancellationToken.None);

        var accepted = invitation.AcceptByUser(invitedUserId);
        accepted.Should().BeTrue();

        // Act
        await _sut.SaveAsync(invitation, CancellationToken.None);
        var loaded = await _sut.GetAsync(invitation.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Status.Should().Be(InvitationStatus.Accepted);
    }

    [Fact]
    public async Task RejectByUser_ShouldPersistRejectedStatus()
    {
        // Arrange
        var spaceId = await CreateSpaceAsync();
        var invitedUserId = UserId.New();
        var invitedBy = UserId.New();
        var createdAt = DateTimeOffset.UtcNow;
        var invitation = Invitation.Create(spaceId, invitedUserId, invitedBy, createdAt);
        await _sut.SaveAsync(invitation, CancellationToken.None);

        var rejected = invitation.RejectByUser(invitedUserId);
        rejected.Should().BeTrue();

        // Act
        await _sut.SaveAsync(invitation, CancellationToken.None);
        var loaded = await _sut.GetAsync(invitation.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Status.Should().Be(InvitationStatus.Rejected);
    }

    private async Task<SpaceId> CreateSpaceAsync()
    {
        var space = Space.CreatePrivate("My Space", UserId.New(), DateTimeOffset.UtcNow);
        await _spaceRepository.SaveAsync(space, CancellationToken.None);
        return space.Id;
    }
}
