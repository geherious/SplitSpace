namespace SplitSpace.SharedKernel.Models;

public enum ErrorType
{
    Validation,
    FailedPrecondition,
    Unauthenticated,
    AlreadyExists,
    NotFound,
    Internal
}
