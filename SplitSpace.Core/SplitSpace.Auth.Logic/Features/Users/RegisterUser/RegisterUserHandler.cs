using Mediator;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.Auth.Logic.Services;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Users.RegisterUser;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, Result<RegisterUserResultData>>
{
    private readonly TimeProvider _timeProvider;
    private readonly PasswordHasher _passwordHasher;
    
    private readonly IAccessTokenService _accessTokenService;
    
    private readonly IUserDomainRepository _userDomainRepository;
    private readonly IRefreshTokenDomainRepository _refreshTokenDomainRepository;

    public RegisterUserHandler(
        TimeProvider timeProvider,
        PasswordHasher passwordHasher,
        IAccessTokenService accessTokenService,
        IUserDomainRepository userDomainRepository,
        IRefreshTokenDomainRepository refreshTokenDomainRepository)
    {
        _timeProvider = timeProvider;
        _passwordHasher = passwordHasher;
        _accessTokenService = accessTokenService;
        _userDomainRepository = userDomainRepository;
        _refreshTokenDomainRepository = refreshTokenDomainRepository;
    }

    public async ValueTask<Result<RegisterUserResultData>> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        var now = _timeProvider.GetUtcNow();
        var userResult = User.Create(command.Email, command.Password, _passwordHasher, now);
        if (userResult.IsSuccess is false)
        {
            return Result<RegisterUserResultData>.Failure(userResult.Errors);
        }

        var user = userResult.ResultValue;

        var existingUser = await _userDomainRepository.GetAsync(user.Email, ct);
        if (existingUser is not null)
        {
            return Result<RegisterUserResultData>.Failure(
                new Error(ErrorType.AlreadyExists, "User with this email already exists"));
        }

        var (refreshToken, rawRefreshToken) = RefreshToken.Create(user.Id, now);
        var accessToken = _accessTokenService.Generate(user);
        
        await _userDomainRepository.SaveAsync(user, ct);
        await _refreshTokenDomainRepository.SaveAsync(refreshToken, ct);

        return Result<RegisterUserResultData>.Success(
            new RegisterUserResultData(user.Id, accessToken, rawRefreshToken));
    }
}
