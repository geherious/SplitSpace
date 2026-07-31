using Mediator;
using SplitSpace.SharedKernel.Domain.Exceptions;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Dal.Database.Transactions;
using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;

namespace SplitSpace.Spaces.Logic.Features.Invitations.AcceptInvitation;

public class AcceptInvitationHandler : ICommandHandler<AcceptInvitationCommand, Result>
{
    private readonly TimeProvider _timeProvider;

    private readonly ITransactionProvider _transactionProvider;
    private readonly IInvitationDomainRepository _invitationDomainRepository;
    private readonly ISpaceDomainRepository _spaceDomainRepository;

    public AcceptInvitationHandler(
        TimeProvider timeProvider,
        ITransactionProvider transactionProvider,
        IInvitationDomainRepository invitationDomainRepository,
        ISpaceDomainRepository spaceDomainRepository)
    {
        _timeProvider = timeProvider;
        _transactionProvider = transactionProvider;
        _invitationDomainRepository = invitationDomainRepository;
        _spaceDomainRepository = spaceDomainRepository;
    }

    public async ValueTask<Result> Handle(AcceptInvitationCommand command, CancellationToken ct)
    {
        var invitation = await _invitationDomainRepository.GetAsync(command.InvitationId, ct);

        if (invitation is null)
        {
            return Result.Failure(new Error(ErrorType.NotFound, $"Invitation with id {command.InvitationId.Value} not found"));
        }

        var space = await _spaceDomainRepository.GetAsync(invitation.SpaceId, ct);

        if (space is null)
        {
            throw new InvariantViolationException("Invitation should be linked to space");
        }

        var acceptResult = invitation.AcceptByUser(command.UserId);
        if (acceptResult is false)
        {
            return Result.Failure(new Error(ErrorType.NotFound, $"Invitation with id {command.InvitationId.Value} can not be accepted"));
        }

        var now = _timeProvider.GetUtcNow();
        space.AddMember(command.UserId, now);

        await using var tx = await _transactionProvider.BeginAsync(ct);
        
        await tx.Repositories.InvitationDomainRepository.SaveAsync(invitation, ct);
        await tx.Repositories.SpaceDomainRepository.SaveAsync(space, ct);
        
        await tx.CommitAsync(ct);

        return Result.Success();
    }
}
