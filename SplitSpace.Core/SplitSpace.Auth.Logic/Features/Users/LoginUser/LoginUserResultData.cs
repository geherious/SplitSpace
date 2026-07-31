using SplitSpace.Auth.Domain.Models.Ids;

namespace SplitSpace.Auth.Logic.Features.Users.LoginUser;

public record LoginUserResultData(
    UserId UserId,
    string AccessToken,
    string RefreshToken);
    