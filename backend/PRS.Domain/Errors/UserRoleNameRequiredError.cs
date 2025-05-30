namespace PRS.Domain.Errors;

public sealed record UserRoleRequiredError() : IDomainError
{
    public string Code => "User.RoleRequired";
    public string Title => "Role required";
    public string Message => "User must have a role.";
}
