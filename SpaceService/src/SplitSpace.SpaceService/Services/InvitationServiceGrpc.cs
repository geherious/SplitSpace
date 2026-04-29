using Grpc.Core;
using SplitSpace.SpaceService.Api;
using SplitSpace.SpaceService.Helpers;
using SplitSpace.SpaceService.Logic.Models.Commands;
using SplitSpace.SpaceService.Logic.Services;

namespace SplitSpace.SpaceService.Services;

public class InvitationServiceGrpc : InvitationService.InvitationServiceBase
{
    private readonly IInvitationService _invitationService;

    public InvitationServiceGrpc(IInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    public override async Task<InviteUserResponse> InviteUser(InviteUserRequest request, ServerCallContext context)
    {
        var result = await _invitationService.InviteUserAsync(
            new InviteUserCommand(request.InvitedUserEmail, request.InvitedByUserId, request.SpaceId));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new InviteUserResponse
        {
            InvitationId = result.Value.InvitationId.ToString()
        };
    }

    public override async Task<AcceptInvitationResponse> AcceptInvitation(AcceptInvitationRequest request, ServerCallContext context)
    {
        var result = await _invitationService.AcceptInvitationAsync(
            new AcceptInvitationCommand(request.InvitationId, request.UserId));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new AcceptInvitationResponse();
    }

    public override async Task<RejectInvitationResponse> RejectInvitation(RejectInvitationRequest request, ServerCallContext context)
    {
        var result = await _invitationService.RejectInvitationAsync(
            new RejectInvitationCommand(request.InvitationId, request.UserId));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new RejectInvitationResponse();
    }
}
