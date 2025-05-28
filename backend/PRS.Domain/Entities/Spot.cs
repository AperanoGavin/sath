using PRS.Domain.Core;
using PRS.Domain.Enums;
using PRS.Domain.Errors;

namespace PRS.Domain.Entities;

public class Spot
{
    public Guid Id { get; init; }
    public string Key { get; init; }
    public IReadOnlyCollection<SpotCapability> Capabilities => _caps.AsReadOnly();
    private readonly List<SpotCapability> _caps;

    private Spot(Guid id, string key, IEnumerable<SpotCapability>? caps = null)
    {
        Id = id;
        Key = key;
        _caps = caps?.ToList() ?? [];
    }

    public static Result<Spot> Create(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<Spot>.Failure(new DomainError(
                "Spot.InvalidKey", "Empty key", "Spot key cannot be empty."));
        }

        return Result<Spot>.Success(new Spot(Guid.NewGuid(), key));
    }

    public Result AddCapability(SpotCapability cap)
    {
        if (_caps.Contains(cap))
        {
            return Result.Failure(new DomainError(
                        "Spot.DuplicateCapability",
                        "Capability already exists",
                        $"Capability '{cap}' is already assigned."));
        }

        _caps.Add(cap);
        return Result.Success();
    }
}
