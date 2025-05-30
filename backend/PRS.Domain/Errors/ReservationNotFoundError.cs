namespace PRS.Domain.Errors;

public sealed record ReservationNotFoundError(Guid Id) : IDomainError
{
    public string Code => "Reservation.NotFound";
    public string Title => "Not found";
    public string Message => $"No reservation with id '{Id}'.";
}
