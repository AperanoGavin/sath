using Microsoft.EntityFrameworkCore;

using PRS.Domain.Entities;
using PRS.Domain.Repositories;
using PRS.Infrastructure.EF.Contexts;

namespace PRS.Infrastructure.EF.Repositories;

internal class EFSpotRepository(AppDbContext ctx) : ISpotRepository
{
    private readonly AppDbContext _ctx = ctx;

    public async Task<Spot?> GetAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Spots
            .Include(s => s.Reservations)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Spot?> GetByKeyAsync(string key, CancellationToken ct = default)
        => await _ctx.Spots.FirstOrDefaultAsync(s => s.Key == key, ct);

    public async Task<ICollection<Spot>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Spots
            .Include(static s => s.Reservations)
            .ToListAsync(ct);

    public Task AddAsync(Spot spot, CancellationToken ct = default)
    {
        _ctx.Spots.Add(spot);

        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var s = await GetAsync(id, ct);
        if (s != null)
        {
            _ctx.Spots.Remove(s);
        }
    }
}
