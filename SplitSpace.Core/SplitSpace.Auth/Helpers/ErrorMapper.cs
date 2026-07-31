using Grpc.Core;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Helpers;

public static class ErrorMapper
{
    private static readonly Dictionary<ErrorType, StatusCode> ErrorStatusMap = new()
    {
        [ErrorType.Validation] = StatusCode.InvalidArgument,
        [ErrorType.FailedPrecondition] = StatusCode.FailedPrecondition,
        [ErrorType.Unauthenticated] = StatusCode.Unauthenticated,
        [ErrorType.AlreadyExists] = StatusCode.AlreadyExists,
        [ErrorType.Internal] = StatusCode.Internal,
    };

    public static RpcException ToRpcException(Error error)
    {
        var statusCode = ErrorStatusMap.TryGetValue(error.Type, out var status)
            ? status
            : throw new ArgumentOutOfRangeException(nameof(error));

        return new RpcException(new Status(statusCode, error.Message));
    }
}
