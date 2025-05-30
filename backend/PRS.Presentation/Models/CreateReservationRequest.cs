namespace PRS.Presentation.Models;

public record CreateReservationRequest
{
    public Guid SpotId { get; init; }
    public Guid UserId { get; init; }
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public bool NeedsCharger { get; init; }
}