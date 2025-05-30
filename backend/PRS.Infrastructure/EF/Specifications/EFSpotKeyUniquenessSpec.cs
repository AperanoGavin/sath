using PRS.Domain.Repositories;
using PRS.Domain.Specifications;

namespace PRS.Infrastructure.EF.Specifications;

internal class EFSpotKeyUniquenessSpec(ISpotRepository spotRepo) : ISpotKeyUniquenessSpec
{
    private readonly ISpotRepository _spotRepo = spotRepo;

    public async Task<bool> IsSatisfiedBy(string key, CancellationToken ct = default)
    {
        var existing = await _spotRepo.GetByKeyAsync(key, ct);
        return existing is null;
    }
}
