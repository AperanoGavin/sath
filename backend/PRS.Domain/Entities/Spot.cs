using PRS.Domain.Enums;

namespace PRS.Domain.Entities;

public class Spot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Key { get; init; }

    public IReadOnlyCollection<SpotCapability> Capabilities => _capabilities.AsReadOnly();
    private readonly IList<SpotCapability> _capabilities = [];

    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
    private readonly IList<Reservation> _reservations = [];
}