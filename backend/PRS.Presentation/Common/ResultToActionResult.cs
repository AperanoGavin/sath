using Microsoft.AspNetCore.Mvc;

using PRS.Domain.Core;
using PRS.Domain.Errors;

namespace PRS.Presentation.Common;

public static class ResultToActionResult
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return result.Error!.ToProblemResult();
    }

    public static IActionResult ToActionResult(
        this Result result,
        Func<IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        return result.Error!.ToProblemResult();
    }

    private static IActionResult ToProblemResult(this IDomainError err)
    {
        var status = err switch
        {
            // 409 Conflict
            SpotDuplicateKeyError => StatusCodes.Status409Conflict,
            ReservationOverlapError => StatusCodes.Status409Conflict,

            // 404 Not Found
            SpotNotFoundError => StatusCodes.Status404NotFound,
            ReservationNotFoundError => StatusCodes.Status404NotFound,
            UserNotFoundError => StatusCodes.Status404NotFound,

            // 400 Bad Request
            SpotInvalidKeyError => StatusCodes.Status400BadRequest,
            SpotDuplicateCapabilityError => StatusCodes.Status400BadRequest,
            SpotMissingCapabilityError => StatusCodes.Status400BadRequest,

            ReservationInvalidPeriodError => StatusCodes.Status400BadRequest,
            ReservationTooLongError => StatusCodes.Status400BadRequest,
            ReservationPastFromError => StatusCodes.Status400BadRequest,
            ReservationCapabilityRequiredError => StatusCodes.Status400BadRequest,
            ReservationInvalidStateError => StatusCodes.Status400BadRequest,
            ReservationTooLateError => StatusCodes.Status400BadRequest,

            UserNameRequiredError => StatusCodes.Status400BadRequest,
            UserEmailRequiredError => StatusCodes.Status400BadRequest,
            UserRoleRequiredError => StatusCodes.Status400BadRequest,

            // 500 Internal Server Error
            UnknownError => StatusCodes.Status500InternalServerError,

            // catch-all for any future errors
            _ => StatusCodes.Status400BadRequest,
        };

        var pd = new ProblemDetails
        {
            Type = $"https://your.api/errors/{err.Code}",
            Title = err.Title,
            Detail = err.Message,
            Status = status
        };

        return new ObjectResult(pd)
        {
            StatusCode = status,
            ContentTypes = { "application/problem+json" }
        };
    }
}