using SplitSpace.FinanceService.Common.Models;

namespace SplitSpace.FinanceService.Common.Exceptions;

public class ValidationException(string message) : BusinessLogicException(BuildError(message))
{
    private static Error BuildError(string message)
    {
        return new Error
        {
            Type = ErrorType.Validation,
            Message = message
        };
    }
}
