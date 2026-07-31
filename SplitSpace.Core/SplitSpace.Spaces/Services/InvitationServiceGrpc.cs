using Grpc.Core;
using Mediator;
using SplitSpace.Spaces.Api;
using SplitSpace.Spaces.Domain.Models.Ids;
using SplitSpace.Spaces.Helpers;
using SplitSpace.Spaces.Logic.Features.Invitations.AcceptInvitation;
using SplitSpace.Spaces.Logic.Features.Invitations.InviteUser;
using SplitSpace.Spaces.Logic.Features.Invitations.RejectInvitation;

namespace SplitSpace.Spaces.Services;

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
            InvitationId = result.ResultValue.InvitationId.ToString()
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
