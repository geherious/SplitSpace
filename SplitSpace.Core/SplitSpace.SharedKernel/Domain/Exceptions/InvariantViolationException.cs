using SplitSpace.SharedKernel.Models;

namespace SplitSpace.SharedKernel.Domain.Exceptions;

public class InvariantViolationException : Exception
{
    public InvariantViolationException(string message) : base(message) { }

    public InvariantViolationException(Error error) : base(error.ToString()) { }
}
