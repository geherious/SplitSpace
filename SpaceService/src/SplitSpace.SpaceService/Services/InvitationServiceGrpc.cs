using Grpc.Core;
using Mediator;
using SplitSpace.SpaceService.Api;
using SplitSpace.SpaceService.Domain.Models.Ids;
using SplitSpace.SpaceService.Helpers;
using SplitSpace.SpaceService.Logic.Features.Invitations.AcceptInvitation;
using SplitSpace.SpaceService.Logic.Features.Invitations.InviteUser;
using SplitSpace.SpaceService.Logic.Features.Invitations.RejectInvitation;

namespace SplitSpace.SpaceService.Services;

public class InvitationServiceGrpc : InvitationService.InvitationServiceBase
{
    private readonly IMediator _mediator;

    public InvitationServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<InviteUserResponse> InviteUser(InviteUserRequest request, ServerCallContext context)
    {
        var command = new InviteUserCommand(
            request.InvitedUserEmail,
            new UserId(Guid.Parse(request.InvitedByUserId)),
            new SpaceId(Guid.Parse(request.SpaceId)));
        
        var result = await _mediator.Send(command, context.CancellationToken);
        
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
        var command = new AcceptInvitationCommand(
            new InvitationId(Guid.Parse(request.InvitationId)),
            new UserId(Guid.Parse(request.UserId)));
            
        var result = await _mediator.Send(command, context.CancellationToken);
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new AcceptInvitationResponse();
    }

    public override async Task<RejectInvitationResponse> RejectInvitation(RejectInvitationRequest request, ServerCallContext context)
    {
        var command = new RejectInvitationCommand(
            new InvitationId(Guid.Parse(request.InvitationId)), 
            new UserId(Guid.Parse(request.UserId)));
        
        var result = await _mediator.Send(command, context.CancellationToken);
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new RejectInvitationResponse();
    }
}
