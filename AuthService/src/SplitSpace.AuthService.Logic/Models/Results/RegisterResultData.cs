namespace SplitSpace.AuthService.Logic.Models.Results;

public record RegisterResultData(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
