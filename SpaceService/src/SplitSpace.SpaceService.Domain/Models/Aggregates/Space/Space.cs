using SplitSpace.SpaceService.Domain.Common;
using SplitSpace.SpaceService.Domain.Exceptions;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

public sealed record Space : AggregateRoot<SpaceId>
{
    public override SpaceId Id { get; protected set; }
    
    public string Name { get; private set; }
    
    public SpaceType Type { get; private set; }
    
    public UserId OwnerId { get; private set; }

    private List<SpaceMember> SpaceMembers { get; set; } = [];
    public IReadOnlyCollection<SpaceMember> Members => SpaceMembers;
    
    public DateTimeOffset CreatedAt { get; init; }

    private Space(
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
        
        return new Space(
            spaceId: spaceId,
            name: name,
            type: SpaceType.Private,
            ownerId: ownerId,
            spaceMembers: [ownerMember],
            createdAt: createdAt);
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
        
        return new Space(
            spaceId: spaceId,
            name: name,
            type: SpaceType.Shared,
            ownerId: ownerId,
            spaceMembers: [ownerMember, .. otherMembers],
            createdAt: createdAt);
    }

    public bool CanBeDeletedBy(UserId userId)
    {
        if (OwnerId != userId)
        {
            return false;
        }
        
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
        
        SpaceMembers.Add(SpaceMember.Create(userId, Id, SpaceMemberRole.Member, joinedAt));
        return true;
    }
}
