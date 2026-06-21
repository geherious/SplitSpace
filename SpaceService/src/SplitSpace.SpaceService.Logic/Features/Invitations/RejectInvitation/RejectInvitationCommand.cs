using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Logic.Features.Invitations.RejectInvitation;

public record RejectInvitationCommand(InvitationId InvitationId, UserId UserId) : ICommand<Result>;
