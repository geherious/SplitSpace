namespace SplitSpace.SpaceService.Logic.Models.Commands;

public record InviteUserCommand(string InvitedUserEmail, string InvitedByUserId, string SpaceId);
