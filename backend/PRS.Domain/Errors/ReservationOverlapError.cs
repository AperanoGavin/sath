namespace PRS.Domain.Errors;

public sealed record ReservationOverlapError(string SpotKey, DateTime From, DateTime To) : IDomainError
{
    public string Code => "Reservation.Overlap";
    public string Title => "Time slot unavailable";
    public string Message => $"Spot '{SpotKey}' is already reserved {From:yyyy-MM-dd}→{To:yyyy-MM-dd}.";
}
