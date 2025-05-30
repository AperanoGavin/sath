namespace PRS.Domain.Errors;

public sealed record ReservationAlreadyCancelledError() : IDomainError
{
    public string Code => "Reservation.AlreadyCancelled";
    public string Title => "Cannot cancel";
    public string Message => "Reservation is already cancelled or expired.";
}
