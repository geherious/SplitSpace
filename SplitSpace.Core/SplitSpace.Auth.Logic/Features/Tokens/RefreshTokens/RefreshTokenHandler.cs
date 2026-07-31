using Mediator;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Tokens.RefreshTokens;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResultData>>
{
    private readonly TimeProvider _timeProvider;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenDomainRepository _refreshTokenDomainRepository;
    private readonly IUserDomainRepository _userDomainRepository;

    public RefreshTokenHandler(TimeProvider timeProvider,
        IAccessTokenService accessTokenService,
        IRefreshTokenDomainRepository refreshTokenDomainRepository,
        IUserDomainRepository userDomainRepository)
    {
        _timeProvider = timeProvider;
        _accessTokenService = accessTokenService;
        _refreshTokenDomainRepository = refreshTokenDomainRepository;
        _userDomainRepository = userDomainRepository;
    }

    public async ValueTask<Result<RefreshTokenResultData>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result<RefreshTokenResultData>.Failure(
                new Error(ErrorType.Unauthenticated, "Refresh token is empty"));
        }

        var requestedToken = await _refreshTokenDomainRepository.GetAsync(command.RefreshToken, cancellationToken);
        if (requestedToken is null)
        {
            return Result<RefreshTokenResultData>.Failure(
                new Error(ErrorType.Unauthenticated, "Refresh token is invalid"));
        }

        var currentTime = _timeProvider.GetUtcNow();
        if (requestedToken.IsValid(currentTime) is false)
        {
            return Result<RefreshTokenResultData>.Failure(
                new Error(ErrorType.Unauthenticated, "Refresh token is invalid"));
        }
        
        requestedToken.Revoke(currentTime);
        
        var user = await _userDomainRepository.GetAsync(requestedToken.UserId, cancellationToken);
        ArgumentNullException.ThrowIfNull(user);
        
        var newAccessToken = _accessTokenService.Generate(user);
        var (newRefreshToken, newRefreshTokenRaw) = RefreshToken.Create(requestedToken.UserId, currentTime);
        
        await _refreshTokenDomainRepository.SaveAsync([requestedToken, newRefreshToken], cancellationToken);

        return Result.Success(new RefreshTokenResultData(newAccessToken, newRefreshTokenRaw));
    }
}
