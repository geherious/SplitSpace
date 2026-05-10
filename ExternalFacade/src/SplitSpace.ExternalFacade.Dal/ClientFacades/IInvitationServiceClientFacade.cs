using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades;

public interface IInvitationServiceClientFacade
{
    Task<InviteUserResponse> InviteUserAsync(
        InviteUserRequest inviteUserRequest,
        CancellationToken cancellationToken);
    
    Task<AcceptInvitationResponse> AcceptInvitationAsync(
        AcceptInvitationRequest acceptInvitationRequest,
        CancellationToken cancellationToken);
    
    Task<RejectInvitationResponse> RejectInvitationAsync(
        RejectInvitationRequest rejectInvitationRequest,
        CancellationToken cancellationToken);
}
