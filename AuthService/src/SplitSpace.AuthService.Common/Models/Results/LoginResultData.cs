namespace SplitSpace.AuthService.Common.Models.Results;

public record LoginResultData(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
