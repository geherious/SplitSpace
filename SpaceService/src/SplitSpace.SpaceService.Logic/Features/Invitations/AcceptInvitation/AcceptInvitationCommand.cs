using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Logic.Features.Invitations.AcceptInvitation;

public record AcceptInvitationCommand(InvitationId InvitationId, UserId UserId) : ICommand<Result>;
