using PRS.Application.Behaviors;
using PRS.Domain.Core;
using PRS.Domain.Errors;

namespace PRS.Application.Common
{
    public static class MediatorResultExtensions
    {
        public static async Task<T> Unwrap<T>(this Task<Result<T>> task)
        {
            var result = await task.ConfigureAwait(false);
            if (result.IsFailure)
            {
                throw new DomainErrorException((DomainError)result.Error!);
            }
            return result.Value;
        }

        public static async Task Unwrap(this Task<Result> task)
        {
            var result = await task.ConfigureAwait(false);
            if (result.IsFailure)
            {
                throw new DomainErrorException((DomainError)result.Error!);
            }
        }
    }
}
