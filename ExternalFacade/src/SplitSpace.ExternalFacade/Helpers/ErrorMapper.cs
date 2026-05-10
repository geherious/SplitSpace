using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SplitSpace.ExternalFacade.Common.Models;

namespace SplitSpace.ExternalFacade.Helpers;

public static class ErrorMapper
{
    private static IActionResult? MapError(Error error)
    {
        var jsonResult = new
        {
            error.Message,
        };
        
        switch (error.Type)
        {
            case ErrorType.Validation:
                return new BadRequestObjectResult(jsonResult);
            case ErrorType.FailedPrecondition:
                return new BadRequestObjectResult(jsonResult);
            case ErrorType.Unauthenticated:
                return new UnauthorizedObjectResult(jsonResult);
            default:
                return null;
        }
    }

    public static IActionResult ToActionResult(this Error error)
    {
        var result = MapError(error);

        if (result is null)
        {
            throw new ArgumentOutOfRangeException(nameof(error.Type));
        }

        return result;
    }
}