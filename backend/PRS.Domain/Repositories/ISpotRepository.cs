using PRS.Domain.Entities;

namespace PRS.Domain.Repositories;

public interface ISpotRepository
{
    Task<Spot?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Spot?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<ICollection<Spot>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Spot spot, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}