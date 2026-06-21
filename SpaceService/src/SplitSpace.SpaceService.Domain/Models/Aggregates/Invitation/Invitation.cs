using SplitSpace.SpaceService.Domain.Common;
using SplitSpace.SpaceService.Domain.Exceptions;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;

public record Invitation : AggregateRoot<InvitationId>
{
    public override InvitationId Id { get; protected set; }
    
    public SpaceId SpaceId { get; private set; }
    
    public UserId InvitedUserId { get; private set; }
    
    public UserId InvitedBy { get; private set; }
    
    public InvitationStatus Status { get; private set; }
    
    public DateTimeOffset ExpiresAt { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }

    public static Invitation Create(SpaceId spaceId,
        UserId invitedUserId,
        UserId invitedBy,
        DateTimeOffset createdAt)
    {
        var expiresAt = createdAt + TimeSpan.FromDays(30);
        
        if (invitedUserId == invitedBy)
        {
            throw new DomainException("Cannot create invitation for the same user");
        }
        
        return new Invitation
        {
            Id = InvitationId.New(),
            SpaceId = spaceId,
            InvitedUserId = invitedUserId,
            InvitedBy = invitedBy,
            Status = InvitationStatus.Created,
            ExpiresAt = expiresAt,
            CreatedAt = createdAt
        };
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
        
        return true;
    }
}
