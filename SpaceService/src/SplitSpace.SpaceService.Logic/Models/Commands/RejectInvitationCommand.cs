namespace SplitSpace.SpaceService.Logic.Models.Commands;

public record RejectInvitationCommand(string InvitationId, string UserId);