namespace PRS.Application.Models;

public record UserDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Email { get; init; } = default!;
    public RoleDto Role { get; init; } = default!;
}
