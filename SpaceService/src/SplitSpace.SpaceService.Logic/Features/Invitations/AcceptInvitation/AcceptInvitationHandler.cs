using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Dal.Repositories;
using SplitSpace.SpaceService.Domain.Exceptions;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Logic.Features.Invitations.AcceptInvitation;

public class AcceptInvitationHandler : ICommandHandler<AcceptInvitationCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvitationDomainRepository _invitationDomainRepository;
    private readonly ISpaceDomainRepository _spaceDomainRepository;

    public AcceptInvitationHandler(
        TimeProvider timeProvider,
        IUnitOfWork unitOfWork,
        IInvitationDomainRepository invitationDomainRepository,
        ISpaceDomainRepository spaceDomainRepository)
    {
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
        _invitationDomainRepository = invitationDomainRepository;
        _spaceDomainRepository = spaceDomainRepository;
    }

    public async ValueTask<Result> Handle(AcceptInvitationCommand command, CancellationToken ct)
    {
        var invitation = await _invitationDomainRepository.GetAsync(command.InvitationId, ct);

        if (invitation is null)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Code = null,
                Message = $"Invitation with id {command.InvitationId.Value} not found"
            });
        }
        
        var space = await _spaceDomainRepository.GetAsync(invitation.SpaceId, ct);

        if (space is null)
        {
            throw new InvariantViolationException("Invitation should be linked to space");
        }

        var acceptResult = invitation.AcceptByUser(command.UserId);
        if (acceptResult is false)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Code = null,
                Message = $"Invitation with id {command.InvitationId.Value} can not be accepted"
            });
        }

        var now = _timeProvider.GetUtcNow();
        space.AddMember(command.UserId, now);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
