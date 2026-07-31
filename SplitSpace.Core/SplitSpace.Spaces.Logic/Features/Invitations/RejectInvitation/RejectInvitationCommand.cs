using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Invitations.RejectInvitation;

public record RejectInvitationCommand(InvitationId InvitationId, UserId UserId) : ICommand<Result>;
