using SplitSpace.Auth.Domain.Models.Events;
using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;

public sealed record User : AggregateRoot<UserId>
{
    public override UserId Id { get; protected set; }
    
    public Email Email { get; private set; }
    
    public PasswordHash PasswordHash { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset? LastLogin { get; private set; }

    private User(
        UserId id,
        Email email,
        PasswordHash passwordHash,
        DateTimeOffset createdAt,
        DateTimeOffset? lastLogin)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        LastLogin = lastLogin;
    }
    
    private User() {}

    public static User Rehydrate(
        UserId id,
        Email email,
        PasswordHash passwordHash,
        DateTimeOffset createdAt,
        DateTimeOffset? lastLogin)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = createdAt,
            LastLogin = lastLogin,
        };
    }

    public static Result<User> Create(
        string email,
        string rawPassword,
        IPasswordHasher passwordHasher,
        DateTimeOffset createdAt)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsSuccess is false)
        {
            return Result<User>.Failure(emailResult.Errors);
        }

        var passwordHashResult = PasswordHash.Create(rawPassword, passwordHasher);
        if (passwordHashResult.IsSuccess is false)
        {
            return Result<User>.Failure(passwordHashResult.Errors);
        }

        var user = new User(
            UserId.New(),
            emailResult.ResultValue,
            passwordHashResult.ResultValue,
            createdAt,
            null);
        
        user.AddDomainEvent(new UserCreatedEvent(user));
        return Result.Success(user);
    }
}
