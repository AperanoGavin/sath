using PRS.Domain.Entities;

namespace PRS.Domain.Repositories;

// TODO: Split into a ReadOnly and RW repository
public interface IReservationRepository
{
    Task<ICollection<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Reservation?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<Reservation>> GetBySpotAsync(Guid spotId, CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}