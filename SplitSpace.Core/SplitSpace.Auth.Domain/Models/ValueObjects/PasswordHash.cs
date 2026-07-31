using SplitSpace.Auth.Domain.Services;
using SplitSpace.SharedKernel.Domain.Exceptions;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Domain.Models.ValueObjects;

public readonly record struct PasswordHash : IEquatable<PasswordHash>
{
    public string Value { get; }

    internal PasswordHash(string hashedValue)
    {
        Value = hashedValue;
    }

    private static Result Validate(string rawPassword)
    {
        if (string.IsNullOrWhiteSpace(rawPassword))
            return Result.Failure(new Error(ErrorType.Validation, "Password cannot be empty"));
        
        if (rawPassword.Length < 8)
            return Result.Failure(
                new Error(ErrorType.Validation, "Password must be at least 8 characters long"));
        
        return Result.Success();
    }

    public static Result<PasswordHash> Create(string rawPassword, IPasswordHasher hasher)
    {
        var result = Validate(rawPassword);
        if (result.IsSuccess is false)
        {
            return Result<PasswordHash>.Failure(result.Errors);
        }
        
        var hashedPassword = hasher.Hash(rawPassword);
        
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new InvariantViolationException("Invalid password hash");

        return Result.Success(new PasswordHash(hashedPassword));
    }

    public bool Verify(string rawPassword, IPasswordHasher hasher)
        => hasher.Verify(rawPassword, Value);

    public bool Equals(PasswordHash? other) => other is not null && Value == other.Value.Value;
}
