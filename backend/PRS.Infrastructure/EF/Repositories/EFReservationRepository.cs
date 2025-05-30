using Microsoft.EntityFrameworkCore;

using PRS.Domain.Entities;
using PRS.Domain.Repositories;
using PRS.Infrastructure.EF.Contexts;

using EntityFramework = Microsoft.EntityFrameworkCore.EF;

namespace PRS.Infrastructure.EF.Repositories;

internal class EFReservationRepository(AppDbContext ctx) : IReservationRepository
{
    private readonly AppDbContext _ctx = ctx;

    public async Task<Reservation?> GetAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Reservations
            .Include(r => r.Spot)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<ICollection<Reservation>> GetBySpotAsync(
        Guid spotId,
        CancellationToken ct = default)
        => await _ctx.Reservations
            .Where(r => EntityFramework.Property<Guid>(r, "SpotId") == spotId)
            .ToListAsync(ct);

    public async Task<ICollection<Reservation>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Reservations
            .Include(static r => r.Spot)
            .ToListAsync(ct);

    public Task AddAsync(Reservation reservation, CancellationToken ct = default)
    {
        _ctx.Reservations.Add(reservation);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var r = await GetAsync(id, ct);
        if (r != null)
        {
            _ctx.Reservations.Remove(r);
        }
    }

    public Task UpdateAsync(Reservation reservation, CancellationToken ct = default)
    {
        _ctx.Reservations.Update(reservation);
        return Task.CompletedTask;
    }
}
