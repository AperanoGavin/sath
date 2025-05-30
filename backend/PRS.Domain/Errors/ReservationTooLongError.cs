namespace PRS.Domain.Errors;

public sealed record ReservationTooLongError(string Detail) : IDomainError
{
    public string Code => "Reservation.TooLong";
    public string Title => "Period too long";
    public string Message => Detail;
}
