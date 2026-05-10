using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades.Implementations;

public class InvitationServiceClientFacade : IInvitationServiceClientFacade
{
    private readonly SpaceService.Api.InvitationService.InvitationServiceClient _invitationServiceClient;

    public InvitationServiceClientFacade(InvitationService.InvitationServiceClient invitationServiceClient)
    {
        _invitationServiceClient = invitationServiceClient;
    }

    public async Task<InviteUserResponse> InviteUserAsync(
        InviteUserRequest inviteUserRequest,
        CancellationToken cancellationToken)
    {
        var response = await _invitationServiceClient.InviteUserAsync(
            inviteUserRequest,
            cancellationToken: cancellationToken);
        
        return response;
    }

    public async Task<AcceptInvitationResponse> AcceptInvitationAsync(
        AcceptInvitationRequest acceptInvitationRequest,
        CancellationToken cancellationToken)
    {
        var response = await _invitationServiceClient.AcceptInvitationAsync(
            acceptInvitationRequest,
            cancellationToken: cancellationToken);
        
        return response;
    }

    public async Task<RejectInvitationResponse> RejectInvitationAsync(RejectInvitationRequest rejectInvitationRequest,
        CancellationToken cancellationToken)
    {
        var response = await _invitationServiceClient.RejectInvitationAsync(
            rejectInvitationRequest,
            cancellationToken: cancellationToken);
        
        return response;
    }
}
