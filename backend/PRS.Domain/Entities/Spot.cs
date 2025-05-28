using PRS.Domain.Core;
using PRS.Domain.Enums;
using PRS.Domain.Errors;

namespace PRS.Domain.Entities;

public class Spot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; init; }

    public IReadOnlyCollection<SpotCapability> Capabilities => _capabilities.AsReadOnly();
    private readonly IList<SpotCapability> _capabilities = [];

    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
    private readonly IList<Reservation> _reservations = [];

    private Spot(string key)
    {
        Key = key;
    }

    public static Result<Spot> Create(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<Spot>.Failure(
                    new DomainError("InvalidKey", "Empty key", "Spot key cannot be empty."));
        }

        return Result<Spot>.Success(new Spot(key));
    }

    public Result AddCapability(SpotCapability cap)
    {
        if (_capabilities.Contains(cap))
        {
            return Result.Failure(
                    new DomainError("DuplicateCapability",
                        "Capability already exists",
                        $"Capability '{cap}' has already been assigned."));
        }

        _capabilities.Add(cap);
        return Result.Success();
    }
}