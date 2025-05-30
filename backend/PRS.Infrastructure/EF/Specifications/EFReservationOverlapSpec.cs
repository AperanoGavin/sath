using PRS.Domain.Entities;
using PRS.Domain.Repositories;
using PRS.Domain.Specifications;

namespace PRS.Infrastructure.EF.Specifications;

internal class EFReservationOverlapSpec(IReservationRepository reservationRepo) : IReservationOverlapSpec
{
    private readonly IReservationRepository _reservationRepo = reservationRepo;

    public async Task<bool> IsSatisfiedBy(
        Spot spot,
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var existing = await _reservationRepo.GetBySpotAsync(spot.Id, ct);
        return !existing.Any(r => r.Overlaps(from, to));
    }
}
