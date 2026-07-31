using SplitSpace.SharedKernel.Domain.Exceptions;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.Spaces.Domain.Models.Events;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;

public sealed record Invitation : AggregateRoot<InvitationId>
{
    public override InvitationId Id { get; protected set; }

    public SpaceId SpaceId { get; private set; }

    public UserId InvitedUserId { get; private set; }

    public UserId InvitedBy { get; private set; }

    public InvitationStatus Status { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private Invitation(
        InvitationId id,
        SpaceId spaceId,
        UserId invitedUserId,
        UserId invitedBy,
        InvitationStatus status,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt)
    {
        if (invitedUserId == invitedBy)
        {
            throw new DomainException("Cannot create invitation for the same user");
        }
        
        Id = id;
        SpaceId = spaceId;
        InvitedUserId = invitedUserId;
        InvitedBy = invitedBy;
        Status = status;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }
    
    private Invitation() {}

    public static Invitation Rehydrate(
        InvitationId id,
        SpaceId spaceId,
        UserId invitedUserId,
        UserId invitedBy,
        InvitationStatus status,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt)
    {
        return new Invitation
        {
            Id = id,
            SpaceId = spaceId,
            InvitedUserId = invitedUserId,
            InvitedBy = invitedBy,
            Status = status,
            ExpiresAt = expiresAt,
            CreatedAt = createdAt
        };
    }

    public static Invitation Create(SpaceId spaceId,
        UserId invitedUserId,
        UserId invitedBy,
        DateTimeOffset createdAt)
    {
        var expiresAt = createdAt + TimeSpan.FromDays(30);

        var id = InvitationId.New();
        var invitation = new Invitation(
            id,
            spaceId,
            invitedUserId,
            invitedBy,
            InvitationStatus.Created,
            expiresAt,
            createdAt);

        invitation.AddDomainEvent(new InvitationCreatedEvent(id, spaceId, invitedUserId, invitedBy, expiresAt, createdAt));

        return invitation;
    }

    public bool AcceptByUser(UserId acceptedBy)
    {
        if (Status != InvitationStatus.Created)
        {
            return false;
        }

        if (InvitedUserId != acceptedBy)
        {
            return false;
        }

        Status = InvitationStatus.Accepted;
        AddDomainEvent(new InvitationAcceptedEvent(Id));

        return true;
    }

    public bool RejectByUser(UserId acceptedBy)
    {
        if (Status != InvitationStatus.Created)
        {
            return false;
        }

        if (InvitedUserId != acceptedBy)
        {
            return false;
        }

        Status = InvitationStatus.Rejected;
        AddDomainEvent(new InvitationRejectedEvent(Id));

        return true;
    }
}
