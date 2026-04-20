namespace SplitSpace.AuthService.Common.Models;

public record Error
{
    public required ErrorType Type { get; set; }
    public required string Code { get; set; }
    public required string Message { get; set; }
}
