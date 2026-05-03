using SplitSpace.FinanceService.Common.Models;

namespace SplitSpace.FinanceService.Common.Exceptions;

public class BusinessLogicException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}
