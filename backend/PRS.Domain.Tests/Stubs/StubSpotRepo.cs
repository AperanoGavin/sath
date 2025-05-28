using PRS.Domain.Entities;
using PRS.Domain.Repositories;

namespace PRS.Domain.Tests.Stubs;

internal class StubSpotRepo(Spot? spot) : ISpotRepository
{
    private readonly Spot? _spot = spot;

    public Task<Spot?> GetAsync(Guid id, CancellationToken _)
        => Task.FromResult(_spot);

    public Task<Spot?> GetByKeyAsync(string key, CancellationToken ct = default)
        => throw new NotImplementedException();
    public Task<ICollection<Spot>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();
    public Task AddAsync(Spot spot, CancellationToken ct = default)
        => throw new NotImplementedException();
    public Task RemoveAsync(Guid id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
