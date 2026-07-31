using System.Diagnostics.CodeAnalysis;

namespace SplitSpace.SharedKernel.Models;

public class Result<T>
{
    [MemberNotNullWhen(true, nameof(ResultValue))]
    public bool IsSuccess => Errors.Count == 0;
    
    public List<Error> Errors { get; }
    
    public Error Error => Errors.First();
    
    public T? ResultValue { get; private set; }

    private Result(List<Error> errors, T? resultValue)
    {
        Errors = errors;
        ResultValue = resultValue;
    }

    public static Result<T> Success(T value) => new([], value);

    public static Result<T> Failure(List<Error> errors) => new(errors, default);
    public static Result<T> Failure(Error error) => new([error], default);
}

public class Result
{
    public bool IsSuccess => Errors.Count == 0;
    
    public List<Error> Errors { get; }
    
    public Error Error => Errors.First();

    private Result(List<Error> errors)
    {
        Errors = errors;
    }

    public static Result Success() => new([]);
    
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result Failure(List<Error> errors) => new(errors);
    public static Result Failure(Error error) => new( [error]);
}
