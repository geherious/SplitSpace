using System.Diagnostics.CodeAnalysis;

namespace SplitSpace.SpaceService.Common.Models;

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
}
