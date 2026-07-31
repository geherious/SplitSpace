using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Invitations.AcceptInvitation;

public record AcceptInvitationCommand(InvitationId InvitationId, UserId UserId) : ICommand<Result>;
