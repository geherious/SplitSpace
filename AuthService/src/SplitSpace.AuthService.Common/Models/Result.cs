using System.Diagnostics.CodeAnalysis;

namespace SplitSpace.AuthService.Common.Models;

public class Result<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; }
    
    public List<Error> Errors { get; }
    
    public T? Value { get; private set; }

    private Result(bool success, List<Error> errors, T? value)
    {
        IsSuccess = success;
        Errors = errors;
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, [], value);

    public static Result<T> Failure(List<Error> errors) => new(false, errors, default);
    public static Result<T> Failure(Error error) => new(false, [error], default);
}
