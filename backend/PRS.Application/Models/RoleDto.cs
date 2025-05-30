namespace PRS.Application.Models;

public record RoleDto
{
    public string Id { get; init; } = default!;
    public string Key { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
}
