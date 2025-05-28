namespace PRS.Presentation.Models;

public record CreateReservationRequest
{
    public string SpotId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public bool IsReservation { get; init; }
}

