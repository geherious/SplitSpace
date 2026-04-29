using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Logic.Models.Commands;
using SplitSpace.SpaceService.Logic.Models.Results;

namespace SplitSpace.SpaceService.Logic.Services;

public interface IInvitationService
{
    Task<Result<InviteUserResultData>> InviteUserAsync(InviteUserCommand command);
    Task<Result> AcceptInvitationAsync(AcceptInvitationCommand command);
    Task<Result> RejectInvitationAsync(RejectInvitationCommand command);
}
