namespace PRS.Domain.Errors;

public sealed record UserEmailRequiredError() : IDomainError
{
    public string Code => "User.EmailRequired";
    public string Title => "Email required";
    public string Message => "User email cannot be empty.";
}