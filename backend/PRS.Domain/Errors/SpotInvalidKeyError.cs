namespace PRS.Domain.Errors;

public sealed record SpotInvalidKeyError() : IDomainError
{
    public string Code => "Spot.InvalidKey";
    public string Title => "Invalid key";
    public string Message => "Spot key cannot be empty.";
}
