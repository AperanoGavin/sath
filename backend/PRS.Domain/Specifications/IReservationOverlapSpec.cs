using PRS.Domain.Entities;

namespace PRS.Domain.Specifications;

public interface IReservationOverlapSpec
{
    Task<bool> IsSatisfiedBy(
        Spot spot,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}
