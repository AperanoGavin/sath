namespace PRS.Domain.Errors;

public sealed record UserNotFoundError(Guid Id) : IDomainError
{
    public string Code => "User.NotFound";
    public string Title => "Not found";
    public string Message => $"No user with id '{Id}'.";
}
