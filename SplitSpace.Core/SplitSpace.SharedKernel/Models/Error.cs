namespace SplitSpace.SharedKernel.Models;

public record Error(ErrorType Type, string Message, string? entity = null, string? code = null)
{
    public override string ToString()
    {
        return $"{Type} error: {Message}";
    }
}
