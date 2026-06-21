using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Dal.ClientFacades.Implementations;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Logic.Features.Invitations.InviteUser;

public class InviteUserHandler : ICommandHandler<InviteUserCommand, Result<InviteUserResultData>>
{
    private readonly TimeProvider _timeProvider;
    
    private readonly IInvitationDomainRepository _invitationDomainRepository;
    private readonly ISpaceDomainRepository _spaceDomainRepository;
    
    private readonly IAuthServiceClientFacade _authServiceClientFacade;

    public InviteUserHandler(
        TimeProvider timeProvider,
        IInvitationDomainRepository invitationDomainRepository,
        ISpaceDomainRepository spaceDomainRepository,
        IAuthServiceClientFacade authServiceClientFacade)
    {
        _timeProvider = timeProvider;
        _invitationDomainRepository = invitationDomainRepository;
        _spaceDomainRepository = spaceDomainRepository;
        _authServiceClientFacade = authServiceClientFacade;
    }

    public async ValueTask<Result<InviteUserResultData>> Handle(InviteUserCommand command, CancellationToken ct)
    {
        var invitedUserId = await _authServiceClientFacade.UserExistAsync(command.InvitedUserEmail);

        if (invitedUserId == null)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = $"User with email {command.InvitedUserEmail} not found."
            });
        }

        var space = await _spaceDomainRepository.GetAsync(command.SpaceId, ct);
        
        if (space is null)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Code = null,
                Message = $"Space with id {command.SpaceId.Value} not found"
            });
        }

        if (space.HasUser(command.InvitedByUserId) is false)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Code = null,
                Message = $"Space with id {command.SpaceId.Value} not found"
            });
        }

        var currentTime = _timeProvider.GetUtcNow();
        var invitation = Invitation.Create(command.SpaceId, invitedUserId.Value, command.InvitedByUserId, currentTime);

        await _invitationDomainRepository.AddAsync(invitation, ct);

        return Result<InviteUserResultData>.Success(new InviteUserResultData(invitation.Id));
    }
}
