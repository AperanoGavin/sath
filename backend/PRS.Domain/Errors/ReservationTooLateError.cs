namespace PRS.Domain.Errors;

public sealed record ReservationTooLateError() : IDomainError
{
    public string Code => "Reservation.TooLate";
    public string Title => "No-show";
    public string Message => "Check-in after 11 AM is forbidden; reservation expired.";
}