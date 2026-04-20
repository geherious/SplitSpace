using SplitSpace.AuthService.Common.Models;
using SplitSpace.AuthService.Dal.Models.Entities;
using SplitSpace.AuthService.Dal.Repositories;

namespace SplitSpace.AuthService.Logic.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtCreator _jwtCreator;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtCreator jwtCreator,
        PasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtCreator = jwtCreator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<RegisterResultData>> Register(RegisterCommand command)
    {
        var errors = new List<Error>();

        // Validate email format
        if (!IsValidEmail(command.Email))
        {
            errors.Add(new Error
            {
                Type = ErrorType.Validation,
                Code = "INVALID_EMAIL",
                Message = "Invalid email format"
            });
        }

        // Validate password length
        if (command.Password.Length < 8)
        {
            errors.Add(new Error
            {
                Type = ErrorType.Validation,
                Code = "PASSWORD_TOO_SHORT",
                Message = "Password must be at least 8 characters"
            });
        }

        if (errors.Any())
            return Result<RegisterResultData>.Failure(errors);

        // Check if user already exists
        var existingUser = await _userRepository.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            errors.Add(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Code = "USER_EXISTS",
                Message = "User with this email already exists"
            });
            return Result<RegisterResultData>.Failure(errors);
        }

        // Hash password and create user
        var hashedPassword = _passwordHasher.Hash(command.Password);
        var user = new User
        {
            Email = command.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateUserAsync(user);

        // Create tokens
        var accessToken = _jwtCreator.CreateAccessToken(user.Id);
        var refreshToken = _jwtCreator.CreateRefreshToken(user.Id);

        await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken);

        return Result<RegisterResultData>.Success(new RegisterResultData(
            user.Id,
            accessToken,
            refreshToken.Token));
    }

    public async Task<Result<LoginResultData>> Login(LoginCommand command)
    {
        var errors = new List<Error>();

        // Validate email format
        if (!IsValidEmail(command.Email))
        {
            errors.Add(new Error
            {
                Type = ErrorType.Validation,
                Code = "INVALID_EMAIL",
                Message = "Invalid email format"
            });
        }

        if (errors.Any())
            return Result<LoginResultData>.Failure(errors);

        // Find user by email
        var user = await _userRepository.FindByEmailAsync(command.Email);
        if (user == null)
        {
            errors.Add(new Error
            {
                Type = ErrorType.Unauthenticated,
                Code = "USER_NOT_FOUND",
                Message = "Invalid email or password"
            });
            return Result<LoginResultData>.Failure(errors);
        }

        // Verify password
        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            errors.Add(new Error
            {
                Type = ErrorType.Unauthenticated,
                Code = "INVALID_PASSWORD",
                Message = "Invalid email or password"
            });
            return Result<LoginResultData>.Failure(errors);
        }

        // Revoke all existing refresh tokens for this user (logout old sessions)
        await RevokeAllUserTokensAsync(user.Id);

        // Update last login timestamp
        await _userRepository.UpdateLastLoginAsync(user.Id);

        // Create new tokens
        var accessToken = _jwtCreator.CreateAccessToken(user.Id);
        var refreshToken = _jwtCreator.CreateRefreshToken(user.Id);
        
        await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken);

        return Result<LoginResultData>.Success(new LoginResultData(
            user.Id,
            accessToken,
            refreshToken.Token));
    }

    public async Task<Result<RefreshTokensResultData>> RefreshTokens(RefreshTokensCommand command)
    {
        var errors = new List<Error>();

        // Validate refresh token format (basic check)
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            errors.Add(new Error
            {
                Type = ErrorType.Validation,
                Code = "EMPTY_REFRESH_TOKEN",
                Message = "Refresh token cannot be empty"
            });
        }

        if (errors.Any())
            return Result<RefreshTokensResultData>.Failure(errors);

        // Validate refresh token signature
        if (!_jwtCreator.ValidateRefreshToken(command.RefreshToken))
        {
            errors.Add(new Error
            {
                Type = ErrorType.Unauthenticated,
                Code = "INVALID_REFRESH_TOKEN",
                Message = "Refresh token is invalid"
            });
            return Result<RefreshTokensResultData>.Failure(errors);
        }

        // Get the refresh token from database
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken);
        
        if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            errors.Add(new Error
            {
                Type = ErrorType.Unauthenticated,
                Code = "TOKEN_EXPIRED",
                Message = "Refresh token has expired"
            });
            return Result<RefreshTokensResultData>.Failure(errors);
        }

        // Revoke the specific refresh token being used (rotating pattern)
        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken.Id);

        // Create new tokens for the same user
        var newAccessToken = _jwtCreator.CreateAccessToken(refreshToken.UserId);
        var newRefreshToken = _jwtCreator.CreateRefreshToken(refreshToken.UserId);
        
        await _refreshTokenRepository.CreateRefreshTokenAsync(newRefreshToken);

        return Result<RefreshTokensResultData>.Success(new RefreshTokensResultData(
            newAccessToken,
            newRefreshToken.Token));
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _refreshTokenRepository.GetAllUserTokensAsync(userId);
        foreach (var token in tokens)
        {
            await _refreshTokenRepository.RevokeRefreshTokenAsync(token.Id);
        }
    }
}
