namespace PRS.Domain.Errors;

public sealed record UserNameRequiredError() : IDomainError
{
    public string Code => "User.NameRequired";
    public string Title => "Name required";
    public string Message => "User name cannot be empty.";
}
