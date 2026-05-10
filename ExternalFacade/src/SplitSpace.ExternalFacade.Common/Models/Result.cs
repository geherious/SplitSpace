using System.Diagnostics.CodeAnalysis;

namespace SplitSpace.ExternalFacade.Common.Models;

public class Result<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Errors.Count == 0;
    
    public List<Error> Errors { get; }
    
    public T? Value { get; private set; }

    private Result(List<Error> errors, T? value)
    {
        Errors = errors;
        Value = value;
    }
    
    public static implicit operator Result<T>(T value)
        => Success(value);

    public static Result<T> Success(T value) => new([], value);

    public static Result<T> Failure(List<Error> errors) => new(errors, default);
    public static Result<T> Failure(Error error) => new([error], default);
}

public class Result
{
    public bool IsSuccess => Errors.Count == 0;
    
    public List<Error> Errors { get; }

    private Result(List<Error> errors)
    {
        Errors = errors;
    }

    public static Result Success() => new([]);

    public static Result Failure(List<Error> errors) => new(errors);
    public static Result Failure(Error error) => new( [error]);
    
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);

    public static Result<T> Failure<T>(List<Error> errors) => Result<T>.Failure(errors);
}
