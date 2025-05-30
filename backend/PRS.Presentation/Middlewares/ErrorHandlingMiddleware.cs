using System.Net;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Behaviors;
using PRS.Domain.Errors;

namespace PRS.Presentation.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (DomainErrorException dex)
        {
            _logger.LogWarning(dex, "Domain error: {Code}", dex.Error.Code);

            // BUG: This will lead to bugs, either move the magic strings into a static constant class
            // or create a class per type of error to make some pattern matching :/
            var status = dex.Error.Code switch
            {
                // 4xx conflicts
                "Spot.DuplicateKey" => HttpStatusCode.Conflict,
                "Reservation.Overlap" => HttpStatusCode.Conflict,

                // 4xx not found
                "Reservation.NotFound" => HttpStatusCode.NotFound,
                "Reservation.SpotNotFound" => HttpStatusCode.NotFound,
                "Reservation.UserNotFound" => HttpStatusCode.NotFound,
                "Spot.NotFound" => HttpStatusCode.NotFound,

                _ => HttpStatusCode.BadRequest
            };

            await WriteProblem(ctx, dex.Error, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var err = new DomainError(
                Code: "UnhandledError",
                Title: "An unexpected error occurred",
                Message: ex.Message
            );
            await WriteProblem(ctx, err, HttpStatusCode.InternalServerError);
        }
    }

    private static Task WriteProblem(
        HttpContext ctx,
        IDomainError err,
        HttpStatusCode code)
    {
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = (int)code;

        var pd = new ProblemDetails
        {
            Status = ctx.Response.StatusCode,
            Type = $"https://localhost:9095/errors/{err.Code}",
            Title = err.Title,
            Detail = err.Message
        };
        return ctx.Response.WriteAsJsonAsync(pd);
    }
}