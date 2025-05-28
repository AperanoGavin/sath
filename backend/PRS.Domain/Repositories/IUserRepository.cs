using PRS.Domain.Entities;

namespace PRS.Domain.Repositories;

public interface IUserReadOnlyRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken = default);
}
