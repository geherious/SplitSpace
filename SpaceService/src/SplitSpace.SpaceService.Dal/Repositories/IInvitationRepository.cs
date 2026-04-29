using SplitSpace.SpaceService.Common.Enums;
using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal.Repositories;

public interface IInvitationRepository
{
    Task AddAsync(Invitation invitation);
    
    Task<Invitation?> GetAsync(Guid invitationId);
    
    Task<Invitation?> GetAsync(Guid spaceId, Guid invitedUserId, InvitationStatus status);
    
    Task<int> AcceptInvitationAsync(Guid invitationId);
    
    Task<int> RejectInvitationAsync(Guid invitationId);
}
