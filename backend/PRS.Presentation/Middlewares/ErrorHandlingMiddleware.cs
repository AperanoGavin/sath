using System.Net;

using Microsoft.AspNetCore.Mvc;

using PRS.Domain.Errors;

namespace PRS.Presentation.Middlewares;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var err = new UnknownError(ex.Message);
            await WriteProblem(ctx, err, HttpStatusCode.InternalServerError);
        }
    }

    private static Task WriteProblem(
        HttpContext ctx,
        IDomainError err,
        HttpStatusCode code)
    {
        ctx.Response.Clear();
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/problem+json";

        var pd = new ProblemDetails
        {
            Type = $"https://your.api/errors/{err.Code}",
            Title = err.Title,
            Detail = err.Message,
            Status = (int)code
        };

        return ctx.Response.WriteAsJsonAsync(pd);
    }
}