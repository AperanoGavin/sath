using PRS.Domain.Core;
using PRS.Domain.Errors;

namespace PRS.Domain.Entities;

public partial class User
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }

    private User() { }

    internal User(Guid id, string name, string email, UserRole role)
    {
        Id = id;
        Name = name;
        Email = email;
        Role = role;
    }


    public static Result<User> Create(string name, string email, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<User>.Failure(new UserNameRequiredError());
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<User>.Failure(new UserEmailRequiredError());
        }

        if (role is null)
        {
            return Result<User>.Failure(new UserRoleRequiredError());
        }

        return Result<User>.Success(new User(Guid.NewGuid(), name, email, role));
    }

}