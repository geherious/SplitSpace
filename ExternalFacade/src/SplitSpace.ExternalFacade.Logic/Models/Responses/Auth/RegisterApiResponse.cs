namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Auth;

public class RegisterApiResponse
{
    public required string UserId { get; set; }
    public required string AccessToken { get; set; }
}
