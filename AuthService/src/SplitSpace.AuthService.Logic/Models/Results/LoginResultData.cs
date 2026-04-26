namespace SplitSpace.AuthService.Logic.Models.Results;

public record LoginResultData(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
