namespace PRS.Domain.Errors;

public sealed record ReservationInvalidStateError(string Detail) : IDomainError
{
    public string Code => "Reservation.InvalidState";
    public string Title => "Invalid state";
    public string Message => Detail;
}
