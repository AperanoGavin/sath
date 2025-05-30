namespace PRS.Domain.Errors;

public sealed record ReservationPastFromError(DateTime Date) : IDomainError
{
    public string Code => "Reservation.PastFrom";
    public string Title => "Start date in the past";
    public string Message => $"Cannot start before {Date:yyyy-MM-dd}.";
}
