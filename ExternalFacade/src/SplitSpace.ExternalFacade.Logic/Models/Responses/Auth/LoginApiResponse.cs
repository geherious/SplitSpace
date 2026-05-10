namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Auth;

public class LoginApiResponse
{
    public required string UserId { get; set; }
    public required string AccessToken { get; set; }
}