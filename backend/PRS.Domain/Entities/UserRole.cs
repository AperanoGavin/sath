namespace PRS.Domain.Entities;

public class UserRole
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}