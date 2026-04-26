namespace SplitSpace.AuthService.Common.Models.Results;

public record RegisterResultData(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
