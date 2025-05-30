using PRS.Domain.Core;
using PRS.Domain.Entities;

namespace PRS.Domain.Factories
{
    public interface IReservationFactory
    {
        Task<Result<Reservation>> Create(
            Guid spotId,
            Guid userId,
            DateTime from,
            DateTime to,
            bool needsCharger = false,
            CancellationToken cancellationToken = default);
    }
}