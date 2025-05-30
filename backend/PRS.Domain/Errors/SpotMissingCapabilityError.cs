using PRS.Domain.Enums;

namespace PRS.Domain.Errors;

public sealed record SpotMissingCapabilityError(SpotCapability Capability) : IDomainError
{
    public string Code => "Spot.DoesNotHaveCapability";
    public string Title => "Capability not assigned";
    public string Message => $"Capability '{Capability}' is not assigned.";
}
