using System.Net.Mail;
using SplitSpace.SharedKernel.Domain.Exceptions;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Domain.Models.ValueObjects;

public readonly record struct Email
{
    public string Value { get; }

    internal Email(string value)
    {
        Value = value;
    }

    private static Result Validate(string value)
    {
        try
        {
            var addr = new MailAddress(value);
            if (addr.Address == value.Trim())
            {
                return Result.Success();
            }
        }
        catch
        {
            return Result.Failure(new Error(ErrorType.Validation, "Invalid email address"));
        }

        return Result.Success();
    }

    public static Result<Email> Create(string value)
    {
        var result = Validate(value);
        if (result.IsSuccess is false)
        {
            return Result<Email>.Failure(result.Errors);
        }

        return Result<Email>.Success(new Email(value.Trim().ToLowerInvariant()));
    }

    public override string ToString() => Value;
}
