using PRS.Domain.Enums;

namespace PRS.Presentation.Models;

public record ReservationDto
{
    public string Id { get; set; } = string.Empty;
    public string SpotId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public ReservationStatus Status { get; set; }
}