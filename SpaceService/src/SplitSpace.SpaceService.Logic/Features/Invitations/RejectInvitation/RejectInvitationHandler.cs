using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;

namespace SplitSpace.SpaceService.Logic.Features.Invitations.RejectInvitation;

public class RejectInvitationHandler : ICommandHandler<RejectInvitationCommand, Result>
{
    private readonly IInvitationDomainRepository _invitationDomainRepository;

    public RejectInvitationHandler(
        IInvitationDomainRepository invitationDomainRepository)
    {
        _invitationDomainRepository = invitationDomainRepository;
    }

    public async ValueTask<Result> Handle(RejectInvitationCommand command, CancellationToken ct)
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

        var rejectResult = invitation.RejectByUser(command.UserId);
        if (rejectResult is false)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Code = null,
                Message = $"Invitation with id {command.InvitationId.Value} can not be rejected"
            });
        }

        await _invitationDomainRepository.SaveAsync(invitation, ct);

        return Result.Success();
    }
}
