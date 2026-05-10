namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Space;

public class InviteUserApiRequest
{
    public required string InvitedUserEmail { get; set; }
    
    public required Guid SpaceId { get; set; }
}
