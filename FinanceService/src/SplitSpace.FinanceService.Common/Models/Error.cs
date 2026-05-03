namespace SplitSpace.FinanceService.Common.Models;

public record Error
{
    public required ErrorType Type { get; set; }
    public string? Code { get; set; }
    public required string Message { get; set; }
}
