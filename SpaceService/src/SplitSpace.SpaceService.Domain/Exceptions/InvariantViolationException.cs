namespace SplitSpace.SpaceService.Domain.Exceptions;

public class InvariantViolationException : Exception
{
    public InvariantViolationException(string message) : base(message)
    {
        
    }
}