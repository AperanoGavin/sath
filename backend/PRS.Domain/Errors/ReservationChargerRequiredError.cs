using PRS.Domain.Enums;

namespace PRS.Domain.Errors;

public sealed record ReservationCapabilityRequiredError(string SpotKey, SpotCapability SpotCapability) : IDomainError
{
    public string Code => $"Reservation.{SpotCapability}Required";
    public string Title => $"{SpotCapability} needed";
    // TODO: Render a more human friendly SpotCapability than the enum label
    public string Message => $"Spot '{SpotKey}' has no ${SpotCapability}.";
}
