using Grpc.Core;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Spaces.Helpers;

public static class ErrorMapper
{
    private static readonly Dictionary<ErrorType, StatusCode> ErrorStatusMap = new()
    {
        [ErrorType.Validation] = StatusCode.InvalidArgument,
        [ErrorType.FailedPrecondition] = StatusCode.FailedPrecondition,
        [ErrorType.Unauthenticated] = StatusCode.Unauthenticated,
        [ErrorType.NotFound] = StatusCode.NotFound
    };

    public static RpcException ToRpcException(Error error)
    {
        var statusCode = ErrorStatusMap.TryGetValue(error.Type, out var status)
            ? status
            : throw new ArgumentOutOfRangeException(nameof(error));

        return new RpcException(new Status(statusCode, error.Message));
    }
}