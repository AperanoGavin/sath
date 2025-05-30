namespace PRS.Domain.Errors;

public sealed record ReservationInvalidPeriodError() : IDomainError
{
    public string Code => "Reservation.InvalidPeriod";
    public string Title => "Bad dates";
    public string Message => "'To' must come after 'From'.";
}
