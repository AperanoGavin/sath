namespace PRS.Domain.Errors;

public sealed record SpotNotFoundError(Guid Id) : IDomainError
{
    public string Code => "Spot.NotFound";
    public string Title => "Not found";
    public string Message => $"No spot with id '{Id}'.";
}
