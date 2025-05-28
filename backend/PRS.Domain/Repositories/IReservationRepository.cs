using PRS.Domain.Entities;

namespace PRS.Domain.Repositories;

public interface IReservationRepository
{
    Task<Reservation?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<Reservation>> GetBySpotAsync(Guid spotId, CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
