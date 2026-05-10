using System.Text.Json;
using Grpc.Core;

namespace SplitSpace.ExternalFacade.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RpcException ex)
        {
            await HandleRpcExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private async Task HandleRpcExceptionAsync(HttpContext context, RpcException ex)
    {
        context.Response.ContentType = "application/json";
        var httpStatusCode = MapGrpcStatusCodeToHttp(ex.StatusCode);
        context.Response.StatusCode = httpStatusCode;

        if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(ex, "gRPC error occurred: {StatusCode} - {Message}", 
                ex.StatusCode, ex.Message);
        }
        
        var trailer = new
        {
            StatusCode = httpStatusCode,
            Message = ex.Status.Detail
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(trailer));
    }

    private async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception in gRPC call");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        var trailer = new
        {
            StatusCode = (int)StatusCode.Internal,
            Message = "Internal server error"
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(trailer));
    }

    private static int MapGrpcStatusCodeToHttp(StatusCode statusCode)
    {
        return statusCode switch
        {
            StatusCode.OK => StatusCodes.Status200OK,
            StatusCode.Cancelled => StatusCodes.Status499ClientClosedRequest,
            StatusCode.Unknown => StatusCodes.Status500InternalServerError,
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
            StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
            StatusCode.ResourceExhausted => StatusCodes.Status429TooManyRequests,
            StatusCode.FailedPrecondition => StatusCodes.Status400BadRequest,
            StatusCode.Aborted => StatusCodes.Status409Conflict,
            StatusCode.OutOfRange => StatusCodes.Status400BadRequest,
            StatusCode.Unimplemented => StatusCodes.Status500InternalServerError,
            StatusCode.Internal => StatusCodes.Status500InternalServerError,
            StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
