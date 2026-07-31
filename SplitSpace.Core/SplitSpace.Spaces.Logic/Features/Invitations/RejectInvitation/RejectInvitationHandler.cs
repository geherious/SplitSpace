using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;

namespace SplitSpace.Spaces.Logic.Features.Invitations.RejectInvitation;

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
            return Result.Failure(new Error(ErrorType.NotFound, $"Invitation with id {command.InvitationId.Value} not found"));
        }

        var rejectResult = invitation.RejectByUser(command.UserId);
        if (rejectResult is false)
        {
            return Result.Failure(new Error(ErrorType.NotFound, $"Invitation with id {command.InvitationId.Value} can not be rejected"));
        }

        await _invitationDomainRepository.SaveAsync(invitation, ct);

        return Result.Success();
    }
}
