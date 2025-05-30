using PRS.Domain.Enums;

namespace PRS.Domain.Errors;

public sealed record SpotDuplicateCapabilityError(SpotCapability Capability) : IDomainError
{
    public string Code => "Spot.DuplicateCapability";
    public string Title => "Capability already exists";
    public string Message => $"Capability '{Capability}' is already assigned.";
}
