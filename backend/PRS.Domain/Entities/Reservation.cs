using PRS.Domain.Enums;

namespace PRS.Domain.Entities;

public class Reservation
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Spot Spot { get; set; }
    public required User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public ReservationStatus Status { get; set; }
}