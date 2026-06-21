using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Logic.Features.Invitations.InviteUser;

public record InviteUserCommand(string InvitedUserEmail, UserId InvitedByUserId, SpaceId SpaceId)
    : ICommand<Result<InviteUserResultData>>;