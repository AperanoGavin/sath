namespace PRS.Domain.Errors;

public sealed record SpotDuplicateKeyError(string Key) : IDomainError
{
    public string Code => "Spot.DuplicateKey";
    public string Title => "Key already exists";
    public string Message => $"Spot with key '{Key}' already exists.";
}
