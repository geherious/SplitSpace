using SplitSpace.FinanceService.Common.Models;

namespace SplitSpace.FinanceService.Common.Helpers;

public static class ErrorDescriber
{
    public static Error InvalidGuid(string guid)
    {
        return new Error
        {
            Type = ErrorType.Validation,
            Message = $"Invalid guid: {guid}"
        };
    }

    public static Error InvalidMoney(string moneyFieldName)
    {
        return new Error
        {
            Type = ErrorType.Validation,
            Message = $"Invalid money format: {moneyFieldName}"
        };
    }
}
