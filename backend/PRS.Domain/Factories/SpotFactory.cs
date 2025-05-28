using PRS.Domain.Core;
using PRS.Domain.Entities;
using PRS.Domain.Enums;
using PRS.Domain.Errors;
using PRS.Domain.Specifications;

namespace PRS.Domain.Factories;

public class SpotFactory(ISpotKeyUniquenessSpec spec) : ISpotFactory
{
    private readonly ISpotKeyUniquenessSpec _uniquenessSpec = spec;

    public async Task<Result<Spot>> Create(string key, ICollection<SpotCapability> capabilities)
    {
        var creation = Spot.Create(key);
        if (creation.IsFailure)
        {
            return creation;
        }

        var spot = creation.Value;

        if (!await _uniquenessSpec.IsSatisfiedBy(key))
        {
            return Result<Spot>.Failure(
                    new DomainError("Spot.DuplicateKey",
                        "Key already exists",
                        $"Spot with key '{key}' already exists."));
        }

        foreach (var cap in capabilities)
        {
            var capResult = spot.AddCapability(cap);
            if (capResult.IsFailure)
            {
                return Result<Spot>.Failure(capResult.Error!);
            }
        }

        return Result<Spot>.Success(spot);
    }
}
