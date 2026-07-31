using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Invitations.InviteUser;

public record InviteUserCommand(string InvitedUserEmail, UserId InvitedByUserId, SpaceId SpaceId)
    : ICommand<Result<InviteUserResultData>>;