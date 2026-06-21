using SplitSpace.SpaceService.Domain.Exceptions;
using SplitSpace.SpaceService.Domain.Models.Common;
using SplitSpace.SpaceService.Domain.Models.Events;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

public sealed record Space : AggregateRoot<SpaceId>
{
    public override SpaceId Id { get; protected set; }

    public string Name { get; private set; }

    public SpaceType Type { get; private set; }

    public UserId OwnerId { get; private set; }

    private List<SpaceMember> SpaceMembers { get; set; } = [];
    public IReadOnlyCollection<SpaceMember> Members => SpaceMembers.AsReadOnly();

    public DateTimeOffset CreatedAt { get; init; }

    internal Space(
        SpaceId spaceId,
        string name,
        SpaceType type,
        UserId ownerId,
        IReadOnlyCollection<SpaceMember> spaceMembers,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvariantViolationException("Space name cannot be null or empty");
        }

        if (spaceMembers.Count == 0)
        {
            throw new InvariantViolationException("Space should at least contain owner");
        }

        Id = spaceId;
        Name = name;
        Type = type;
        OwnerId = ownerId;
        SpaceMembers = [.. spaceMembers];
        CreatedAt = createdAt;
    }

    public static Space CreatePrivate(string name,
        UserId ownerId,
        DateTimeOffset createdAt)
    {
        var spaceId = SpaceId.New();
        var ownerMember = SpaceMember.Create(ownerId,
            spaceId,
            SpaceMemberRole.Owner,
            createdAt);

        var space = new Space(
            spaceId: spaceId,
            name: name,
            type: SpaceType.Private,
            ownerId: ownerId,
            spaceMembers: [ownerMember],
            createdAt: createdAt);

        space.AddDomainEvent(new SpaceCreatedEvent(spaceId, name, SpaceType.Private, ownerId, createdAt));
        space.AddDomainEvent(new SpaceMemberAddedEvent(ownerMember.Id, spaceId, ownerId, SpaceMemberRole.Owner, createdAt));

        return space;
    }

    public static Space CreateShared(string name,
        UserId ownerId,
        IReadOnlyCollection<UserId> otherMemberIds,
        DateTimeOffset createdAt)
    {
        var spaceId = SpaceId.New();

        var ownerMember = SpaceMember.Create(ownerId,
            spaceId,
            SpaceMemberRole.Owner,
            createdAt);

        var otherMembers = otherMemberIds.Select(mid =>
            SpaceMember.Create(mid, spaceId, SpaceMemberRole.Member, createdAt));

        var allMembers = new[] { ownerMember }.Concat(otherMembers).ToList();

        var space = new Space(
            spaceId: spaceId,
            name: name,
            type: SpaceType.Shared,
            ownerId: ownerId,
            spaceMembers: allMembers,
            createdAt: createdAt);

        space.AddDomainEvent(new SpaceCreatedEvent(spaceId, name, SpaceType.Shared, ownerId, createdAt));
        foreach (var member in allMembers)
        {
            space.AddDomainEvent(new SpaceMemberAddedEvent(member.Id, spaceId, member.UserId, member.Role, member.JoinedAt));
        }

        return space;
    }

    public bool DeleteBy(UserId userId)
    {
        if (OwnerId != userId)
        {
            return false;
        }

        AddDomainEvent(new SpaceDeletedEvent(Id));
        return true;
    }

    public bool HasUser(UserId userId)
    {
        return SpaceMembers.Any(member => member.UserId == userId);
    }

    public bool AddMember(UserId userId, DateTimeOffset joinedAt)
    {
        if (SpaceMembers.Any(m => m.UserId == userId))
        {
            return false;
        }

        var member = SpaceMember.Create(userId, Id, SpaceMemberRole.Member, joinedAt);
        SpaceMembers.Add(member);
        AddDomainEvent(new SpaceMemberAddedEvent(member.Id, Id, userId, SpaceMemberRole.Member, joinedAt));

        return true;
    }

    public void SetMembers(IReadOnlyCollection<SpaceMember> members)
    {
        SpaceMembers = [.. members];
    }
}
