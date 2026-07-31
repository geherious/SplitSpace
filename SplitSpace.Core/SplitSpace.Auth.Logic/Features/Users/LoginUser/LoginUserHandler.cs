using Mediator;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Models.ValueObjects;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.Auth.Logic.Services;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Users.LoginUser;

public class LoginUserHandler : ICommandHandler<LoginUserCommand, Result<LoginUserResultData>>
{
    private readonly TimeProvider _timeProvider;
    
    private readonly IUserDomainRepository _userDomainRepository;
    private readonly IRefreshTokenDomainRepository _refreshTokenDomainRepository;

    private readonly PasswordHasher _passwordHasher;
    private readonly IAccessTokenService _accessTokenService;

    public LoginUserHandler(TimeProvider timeProvider,
        IUserDomainRepository userDomainRepository,
        IRefreshTokenDomainRepository refreshTokenDomainRepository,
        PasswordHasher passwordHasher,
        IAccessTokenService accessTokenService)
    {
        _timeProvider = timeProvider;
        _userDomainRepository = userDomainRepository;
        _refreshTokenDomainRepository = refreshTokenDomainRepository;
        _passwordHasher = passwordHasher;
        _accessTokenService = accessTokenService;
    }

    public async ValueTask<Result<LoginUserResultData>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsSuccess is false)
        {
            return Result<LoginUserResultData>.Failure(emailResult.Errors);
        }
        
        var email = emailResult.ResultValue;
        var user = await _userDomainRepository.GetAsync(email, cancellationToken);

        if (user is null ||
            user.PasswordHash.Verify(command.Password, _passwordHasher) is false)
        {
            return Result<LoginUserResultData>.Failure(
                new Error(ErrorType.Unauthenticated, "Invalid email or password"));
        }

        var now = _timeProvider.GetUtcNow();
        
        var (refreshToken, rawRefreshToken) = RefreshToken.Create(user.Id, now);
        var accessToken = _accessTokenService.Generate(user);
        
        await _refreshTokenDomainRepository.SaveAsync(refreshToken, cancellationToken);

        return Result<LoginUserResultData>.Success(
            new LoginUserResultData(user.Id, accessToken, rawRefreshToken));
    }
}
