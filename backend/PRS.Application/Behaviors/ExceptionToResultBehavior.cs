using System.Reflection;

using MediatR;

using PRS.Domain.Core;
using PRS.Domain.Errors;

namespace PRS.Application.Behaviors;

internal class ExceptionToResultBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TResponse : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        try
        {
            return await next(ct);
        }
        catch (DomainErrorException dex)
        {
            // if you throw a DomainErrorException in your handlers
            return CreateFailureResult(dex.Error);
        }
        catch (Exception ex)
        {
            // unexpected
            var err = new DomainError("UnhandledError", "Unexpected error", ex.Message);
            return CreateFailureResult(err);
        }
    }

    // reflection hack: TResponse is Result<U> or Result (void)
    private static TResponse CreateFailureResult(DomainError error)
    {
        var respType = typeof(TResponse);

        // TResponse = Result<T>
        if (respType.IsGenericType && respType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var factory = respType.GetMethod(
                nameof(Result<object>.Failure),
                BindingFlags.Public | BindingFlags.Static)!
              .MakeGenericMethod(respType.GetGenericArguments()[0]);

            return (TResponse)factory.Invoke(null, [error])!;
        }

        // TResponse = Result
        if (respType == typeof(Result))
        {
            var factory = typeof(Result)
              .GetMethod(nameof(Result.Failure), BindingFlags.Public | BindingFlags.Static)!;
            return (TResponse)factory.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Cannot create a failure {respType}");
    }
}

// TODO: Move this outta here !!!
public class DomainErrorException(DomainError error) : Exception(error.Message)
{
    public DomainError Error { get; } = error;
}
